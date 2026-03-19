namespace IdleFactory.Data.Energy
{
  public class GridItem
  {
    private List<IBuff> buffs = [];

    private IReadOnlyList<ICustomObservable>? observables;

    public GridItem()
    {
      if (this is IBuff buff)
      {
        this.buffs.Add(buff);
      }
    }

    public IReadOnlyList<ICustomObservable> Observables => this.observables ??= [.. this.CollectObservables()];

    protected virtual IEnumerable<ICustomObservable> CollectObservables()
    {
      return [];
    }

    public bool PlacedInGrid { get; set; }

    public Vector2 Position { get; set; } = new Vector2(0, 0);

    public virtual IEnumerable<IBuff> Buffs => this.buffs;

    public virtual bool LaserPassThrough(Laser laser)
    {
      return false;
    }
  }
}
