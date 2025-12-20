

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
      var currentLaser = new Laser(currentPosition, currentPosition, false, laserEmitter.LaserStrength);
      for (var i = 0; i < laserEmitter.MaxDistance; i++)
      {
        var nextPosition = currentPosition + currentDirection;
        if (!IsFree(nextPosition))
        {
          currentLaser = currentLaser with
          {
            To = currentPosition,
            HitTarget = true,
          };
          this.CalculatedLaser.Add(currentLaser);
          return;
        }

        currentPosition = nextPosition;
      }

      currentLaser = currentLaser with { To = currentPosition };
      this.CalculatedLaser.Add(currentLaser);
    }

    private bool IsFree(Vector2 nextPosition)
    {
      if (nextPosition.X < 0 || nextPosition.X >= this.Width || nextPosition.Y < 0 || nextPosition.Y >= this.Height)
      {
        return false;
      }

      return !this.Items.Any(x => x.Position == nextPosition);
    }
  }
}
