using System.Diagnostics;

namespace IdleFactory.Data
{
  public enum ResourceType
  {
    Red = 1,
  }

  public class MainFactory
  {
    public IDictionary<ResourceType, LargeInteger> Resources { get; } = new Dictionary<ResourceType, LargeInteger>();

    public IList<ResourceGenerator> ResourceGenerators { get; } = [];

    public IList<MainFactoryUnlocks> Unlocks { get; } = [];

    public event EventHandler? PropertyChanged;

    private bool hasPropertyChanged;

    public void AfterGameTick()
    {
      if (this.hasPropertyChanged)
      {
        this.PropertyChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    public void Add(ResourceType resourceType, LargeInteger value)
    {
      if (this.Resources.TryGetValue(resourceType, out var currentValue))
      {
        this.Resources[resourceType] = currentValue + value;
      }
      else
      {
        this.Resources[resourceType] = value;
      }

      this.hasPropertyChanged = true;
    }

    public void Remove(ResourceCost cost)
    {
      if (this.Resources.TryGetValue(cost.ResourceType, out var currentValue))
      {
        if (currentValue < cost.Amount)
        {
          Debug.Fail("Not enougth resources");
        }

        this.Resources[cost.ResourceType] = currentValue - cost.Amount;
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
        if (!this.Resources.TryGetValue(cost.ResourceType, out var amount) || amount < cost.Amount)
        {
          return false;
        }
      }

      return true;
    }
  }
}
