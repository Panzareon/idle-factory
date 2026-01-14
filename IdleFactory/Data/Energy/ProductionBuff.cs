using IdleFactory.Data.Main;

namespace IdleFactory.Data.Energy
{
  public class ProductionBuff(ResourceType resourceType) : PoweredItem, IMainFactoryBuff
  {
    public int Order { get; } = 0;

    public override IEnumerable<IBuff> Buffs => [this];

    public string Type => resourceType.ToString().ToLowerInvariant();

    public LargeInteger AdjustProduction(LargeInteger baseAmount, ResourceGenerator resourceGenerator)
    {
      if (resourceGenerator.ResourceType == resourceType)
      {
        return baseAmount * (this.LastPowerValue + 1);
      }

      return baseAmount;
    }
  }
}
