using IdleFactory.Data.Main;

namespace IdleFactory.Data.Energy
{
  public class ProductionEfficiencyBuff(ResourceType resourceType, ResourceType convertFrom) : PoweredItem, IMainFactoryBuff
  {
    public int Order { get; }

    public string Type => resourceType.ToString().ToLowerInvariant();

    public LargeInteger AdjustProduction(LargeInteger baseAmount, ResourceGenerator resourceGenerator)
    {
      return baseAmount;
    }

    public float AdjustProductionConversionFactor(float baseAmount, ResourceGenerator resourceGenerator)
    {
      if (resourceGenerator.ResourceType == resourceType && resourceGenerator.ConvertFrom == convertFrom)
      {
        if (this.LastPowerValue > 100)
        {
          return 0;
        }

        return baseAmount * ((float)this.LastPowerValue * 5 + 1);
      }

      return baseAmount;
    }
  }
}
