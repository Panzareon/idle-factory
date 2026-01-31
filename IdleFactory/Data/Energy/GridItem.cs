namespace IdleFactory.Data.Energy
{
  public class GridItem
  {
    private List<IBuff> buffs = [];

    public GridItem()
    {
      if (this is IBuff buff)
      {
        this.buffs.Add(buff);
      }
    }

    public bool PlacedInGrid { get; set; }

    public Vector2 Position { get; set; } = new Vector2(0, 0);

    public virtual IEnumerable<IBuff> Buffs => this.buffs;
  }
}
