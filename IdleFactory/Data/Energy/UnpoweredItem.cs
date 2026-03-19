namespace IdleFactory.Data.Energy
{
  public class UnpoweredItem : GridItem, IPowerSinkGridItem
  {
    public required LargeInteger RequiredPower { get; set; }

    public BehaviorSubject<LargeInteger> Power { get; } = new() { Value = 0 };

    /// <summary>
    /// Gets the grid item that will be build when this if powered up.
    /// </summary>
    public required GridItem BuildTarget { get; init; }

    /// <summary>
    /// Gets the buildable item, from which this was created.
    /// </summary>
    public required BuildableItem BuildableItem { get; set; }

    /// <inheritdoc/>
    public void HitByLaser(EnergyGrid energyGrid, Laser? laser, LargeInteger strength, float numberOfHits)
    {
      this.Power.Value += strength * numberOfHits;
    }

    /// <inheritdoc/>
    /// <remarks>Nothing to do here.</remarks>
    public void NextTick()
    {
    }

    protected override IEnumerable<ICustomObservable> CollectObservables()
    {
      yield return this.Power;
    }
  }
}
