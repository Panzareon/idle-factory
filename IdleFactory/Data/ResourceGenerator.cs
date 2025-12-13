
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

    public void Upgrade()
    {
      this.UpgradeAmountLevel++;
      this.GenerationAmount *= 2;
    }

    public string GetUpgradeCostString()
    {
      return string.Join(" + ", this.GetUpgradeCost().Select(x => $"{x.Amount} {x.ResourceType}"));
    }

    public IEnumerable<ResourceCost> GetUpgradeCost()
    {
      yield return new ResourceCost(ResourceType.Red, ((LargeInteger)2).ToThePower(this.UpgradeAmountLevel));
    }
  }
}
