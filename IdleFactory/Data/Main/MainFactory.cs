using System.Diagnostics;

namespace IdleFactory.Data.Main
{
  public enum ResourceType
  {
    Undefined = 0,
    Red = 1,
    Blue = 2,
  }

  public class MainFactory
  {
    public MainFactory()
    {
      this.observableResources = new() { Value = this.Resources };
    }
    public IDictionary<ResourceType, Resource> Resources { get; } = new Dictionary<ResourceType, Resource>();

    private readonly BehaviorSubject<IDictionary<ResourceType, Resource>> observableResources;

    public CustomObservableCollection<ResourceGenerator> ResourceGenerators { get; } = new([]);

    public IList<MainFactoryUnlocks> Unlocks { get; } = [];

    public event EventHandler? PropertyChanged;

    private bool hasPropertyChanged;

    public void AfterGameTick()
    {
      if (this.hasPropertyChanged)
      {
        this.PropertyChanged?.Invoke(this, EventArgs.Empty);
        this.hasPropertyChanged = false;
      }

      foreach (var resource in this.Resources.Values)
      {
        resource.AfterGameTick();
      }
    }

    public void Add(ResourceType resourceType, LargeInteger value)
    {
      if (!this.Resources.TryGetValue(resourceType, out var currentValue))
      {
        this.hasPropertyChanged = true;
        currentValue = new Resource();
        this.Resources[resourceType] = currentValue;
        this.observableResources.Value = this.Resources;
      }

      currentValue.Amount += value;
    }

    public void Remove(ResourceCost cost)
    {
      if (this.Resources.TryGetValue(cost.ResourceType, out var currentValue))
      {
        if (currentValue.Amount < cost.Amount)
        {
          Debug.Fail("Not enougth resources");
        }

        currentValue.Amount = currentValue.Amount - cost.Amount;
      }
    }

    public void Remove(IEnumerable<ResourceCost> costs)
    {
      foreach (var cost in costs)
      {
        this.Remove(cost);
      }
    }

    public bool HasResources(IEnumerable<ResourceCost> costs)
    {
      foreach (var cost in costs)
      {
        if (!this.Resources.TryGetValue(cost.ResourceType, out var resource) || resource.Amount < cost.Amount)
        {
          return false;
        }
      }

      return true;
    }

    public ICustomObservable<bool> HasResourcesObservable(IEnumerable<ResourceCost> costs)
    {
      var hasIndividualResources = costs.Select(cost =>
        this.HasResourceObservable(cost.ResourceType, cost.Amount));
      return CustomObservable.CombineLatest(hasIndividualResources, results => results.All(r => r));
    }

    public ICustomObservable<bool> HasResourceObservable(ResourceType resourceType, LargeInteger cost)
    {
      return this.observableResources.SelectMany(resources =>
                resources.TryGetValue(resourceType, out var resource) ?
                resource.ObservableAmount
                    .Select(amount => amount >= cost)
        : CustomObservable.Return(false));
    }

    public bool HasResource(ResourceType convertFrom, LargeInteger cost)
    {
      return this.Resources.TryGetValue(convertFrom, out var resource) && resource.Amount >= cost;
    }

    public LargeInteger GetResource(ResourceType convertFrom)
    {
      return this.Resources.TryGetValue(convertFrom, out var resource) ? resource.Amount : 0;
    }
  }
}
