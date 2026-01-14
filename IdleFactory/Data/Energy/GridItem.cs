namespace IdleFactory.Data.Energy
{
  public class GridItem
  {
    public bool PlacedInGrid { get; set; }

    public Vector2 Position { get; set; } = new Vector2(0, 0);

    public virtual IEnumerable<IBuff> Buffs { get; } = [];
  }
}
