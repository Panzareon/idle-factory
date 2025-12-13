
namespace IdleFactory.Data
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

    public int UpgradeAmountLevel { get; set; } = 1;

    public bool IsFocused { get; set; }

    public void Upgrade()
    {
      this.UpgradeAmountLevel++;
      this.GenerationAmount *= 1.5;
    }

    public string GetUpgradeCostString()
    {
      return this.GetUpgradeCost().ToCostString();
    }

    public IEnumerable<ResourceCost> GetUpgradeCost()
    {
      yield return new ResourceCost(ResourceType.Red, ((LargeInteger)2).ToThePower(this.UpgradeAmountLevel));
    }
  }
}
