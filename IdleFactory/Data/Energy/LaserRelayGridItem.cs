namespace IdleFactory.Data.Energy
{
  public class LaserRelayGridItem : GridItem, IPowerSinkGridItem
  {
    private bool wasHitByLaser;

    public BehaviorSubject<bool> Active { get; } = new() { Value = false };

    public bool Vertical { get; set; }

    public override bool LaserPassThrough(Laser laser)
    {
      return this.Vertical == (laser.Direction.X == 0);
    }

    public void HitByLaser(EnergyGrid energyGrid, Laser? laser, LargeInteger strength, float numberOfHits)
    {
      if (laser == null)
      {
        // This was triggered by another relay, which is not forwarded.
        return;
      }

      IEnumerable<Vector2> positions = [];
      if (this.Vertical)
      {
        if (laser.Direction.X == 0)
        {
          positions = [new Vector2(this.Position.X + 1, this.Position.Y), new Vector2(this.Position.X - 1, this.Position.Y)];
        }
      }
      else
      {
        if (laser.Direction.Y == 0)
        {
          positions = [new Vector2(this.Position.X, this.Position.Y + 1), new Vector2(this.Position.X, this.Position.Y - 1)];
        }
      }

      foreach (var position in positions)
      {
        this.wasHitByLaser = true;
        var gridItem = energyGrid.GetGridItem(position);
        if (energyGrid.GetGridItem(position) is IPowerSinkGridItem powerSinkGridItem)
        {
          powerSinkGridItem.HitByLaser(energyGrid, null, strength, numberOfHits);
        }
      }
    }

    public void NextTick()
    {
      this.Active.Value = this.wasHitByLaser;
      this.wasHitByLaser = false;
    }

    protected override IEnumerable<ICustomObservable> CollectObservables()
    {
      yield return this.Active;
    }
  }
}
