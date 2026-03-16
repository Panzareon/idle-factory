namespace IdleFactory.Data.Energy
{
  public interface IPowerSinkGridItem
  {
    /// <summary>
    /// Gets called when this grid item is hit by a laser.
    /// </summary>
    /// <param name="energyGrid">The parent energy grid.</param>
    /// <param name="laser">The laser that has hit this. Is null if it is not a laser but e.g. a relay.</param>
    /// <param name="strength">The strength of the laser.</param>
    /// <param name="numberOfHits">The number of ticks that this was active.</param>
    void HitByLaser(EnergyGrid energyGrid, Laser? laser, LargeInteger strength, float numberOfHits);
  }
}
