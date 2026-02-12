namespace IdleFactory.Data.Energy
{
  public class GridItem
  {
    private List<IBuff> buffs = [];

    private bool hasChanged = false;

    public GridItem()
    {
      if (this is IBuff buff)
      {
        this.buffs.Add(buff);
      }
    }

    public event EventHandler? PropertyChanged;

    public bool PlacedInGrid { get; set; }

    public Vector2 Position { get; set; } = new Vector2(0, 0);

    public virtual IEnumerable<IBuff> Buffs => this.buffs;

    protected void ValueHasChanged()
    {
      this.hasChanged = true;
    }

    public void AfterGameTick()
    {
      if (this.hasChanged)
      {
        this.PropertyChanged?.Invoke(this, EventArgs.Empty);
        this.hasChanged = false;
      }
    }
  }
}
