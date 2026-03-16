namespace IdleFactory.Data.Energy
{
  public class PoweredItem : GridItem, IPowerSinkGridItem
  {
    public LargeInteger LastPowerValue { get; set; }

    public LargeInteger CurrentPowerValue { get; set; }

    /// <inheritdoc/>
    public void HitByLaser(EnergyGrid energyGrid, Laser laser, LargeInteger strength, float numberOfHits)
    {
      this.CurrentPowerValue += strength;
    }
  }
}
