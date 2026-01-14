namespace IdleFactory.Data.Energy
{
  public class PoweredItem : GridItem, IPowerSinkGridItem
  {
    public LargeInteger LastPowerValue { get; set; }

    public LargeInteger CurrentPowerValue { get; set; }

    public void HitByLaser(LargeInteger strength, float numberOfHits)
    {
      this.CurrentPowerValue += strength;
    }
  }
}
