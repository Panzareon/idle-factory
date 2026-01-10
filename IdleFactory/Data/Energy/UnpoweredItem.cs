namespace IdleFactory.Data.Energy
{
  public class UnpoweredItem : GridItem, IPowerSinkGridItem
  {
    public required LargeInteger RequiredPower { get; set; }

    public LargeInteger Power { get; set; }

    /// <summary>
    /// Gets the grid item that will be build when this if powered up.
    /// </summary>
    public required GridItem BuildTarget { get; init; }

    public void HitByLaser(LargeInteger strength, float numberOfHits)
    {
      this.Power += strength * numberOfHits;
    }
  }
}
