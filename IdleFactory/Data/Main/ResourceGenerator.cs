namespace IdleFactory.Data.Main
{
  public class ResourceGenerator
  {
    public ResourceType ResourceType { get; set; }

    /// <summary>
    /// Gets the time in seconds, how long it takes to genrate the <see cref="GenerationAmount"/>.
    /// </summary>
    public float GenerationTime { get; set; }

    public float ToNextGeneration { get; set; }

    public LargeInteger GenerationAmount { get; set; }

    public BehaviorSubject<int> UpgradeAmountLevel { get; } = new BehaviorSubject<int> { Value = 1 };

    public bool IsFocused { get; set; }

    public bool IsEnabled { get; set; } = true;

    public ResourceType ConvertFrom { get; set; } = ResourceType.Undefined;

    public float ConversionFactor { get; set; }

    public void Upgrade()
    {
      this.UpgradeAmountLevel.Value++;
      if (this.UpgradeAmountLevel.Value < 10)
      {
        this.GenerationTime /= 1.5f;
      }
      else
      {
        this.GenerationAmount *= 1.5;
      }
    }

    public string GetUpgradeCostString()
    {
      return this.GetUpgradeCost().ToCostString();
    }

    public IEnumerable<ResourceCost> GetUpgradeCost()
    {
      if (this.ResourceType == ResourceType.Red)
      {
        yield return new ResourceCost(ResourceType.Red, ((LargeInteger)2).ToThePower(this.UpgradeAmountLevel.Value));
      }
      else if (this.ResourceType == ResourceType.Blue)
      {
        yield return new ResourceCost(ResourceType.Red, 2000 * ((LargeInteger)2).ToThePower(this.UpgradeAmountLevel.Value));
        yield return new ResourceCost(ResourceType.Blue, ((LargeInteger)2).ToThePower(this.UpgradeAmountLevel.Value));
      }
    }
  }
}
