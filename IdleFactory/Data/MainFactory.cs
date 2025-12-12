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

    public event EventHandler? PropertyChanged;

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

      this.PropertyChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
