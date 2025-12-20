

namespace IdleFactory.Data.Energy
{
  public class EnergyGrid
  {
    public bool IsEnabled { get; set; }

    public int Width { get; set; } = 4;

    public int Height { get; set; } = 4;

    public List<Laser> CalculatedLaser { get; } = [];

    public List<GridItem> Items { get; } = [];

    public void AddGridItem(GridItem item)
    {
      this.Items.Add(item);
      this.RecalculateLaser();
    }

    public void RemoveGridItem(GridItem item)
    {
      this.Items.Remove(item);
      this.RecalculateLaser();
    }

    private void RecalculateLaser()
    {
      this.CalculatedLaser.Clear();
      foreach (var laserEmitter in this.Items.OfType<LaserEmitter>())
      {
        this.CalculateLaserEmitter(laserEmitter);
      }
    }

    private void CalculateLaserEmitter(LaserEmitter laserEmitter)
    {
      var currentDirection = laserEmitter.Direction;
      var currentPosition = laserEmitter.Position;
      var currentLaser = new Laser(currentDirection, currentPosition, currentPosition, false, laserEmitter.LaserStrength);
      for (var i = 0; i < laserEmitter.MaxDistance; i++)
      {
        var nextPosition = currentPosition + currentDirection;
        var (isFree, item) = Check(nextPosition);
        if (!isFree)
        {
          switch (item)
          {
            case Mirror mirror:
              this.CalculatedLaser.Add(currentLaser with
              {
                To = nextPosition,
              });
              currentDirection = mirror.PositiveDirection
                ? new Vector2(-currentDirection.Y, -currentDirection.X)
                : new Vector2(currentDirection.Y, currentDirection.X);
              currentLaser = new Laser(currentDirection, nextPosition, nextPosition, false, currentLaser.Strength);
              break;
            default:
              currentLaser = currentLaser with
              {
                To = currentPosition,
                HitTarget = true,
              };
              this.CalculatedLaser.Add(currentLaser);
              return;
          }
        }

        currentPosition = nextPosition;
      }

      currentLaser = currentLaser with { To = currentPosition };
      this.CalculatedLaser.Add(currentLaser);
    }

    private (bool IsFree, GridItem? Item) Check(Vector2 nextPosition)
    {
      if (nextPosition.X < 0 || nextPosition.X >= this.Width || nextPosition.Y < 0 || nextPosition.Y >= this.Height)
      {
        return (false, null);
      }

      var item = this.Items.FirstOrDefault(x => x.Position == nextPosition);
      return (item == null, item);
    }
  }
}
