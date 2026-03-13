namespace IdleFactory.Data.Main
{
  public interface IMainFactoryBuff : IBuff
  {
    public LargeInteger AdjustProduction(LargeInteger baseAmount, ResourceGenerator resourceGenerator);
    public float AdjustProductionConversionFactor(float baseAmount, ResourceGenerator resourceGenerator);
  }
}
