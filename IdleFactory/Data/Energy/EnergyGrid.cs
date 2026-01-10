


namespace IdleFactory.Data.Energy
{
  public class EnergyGrid
  {
    public bool IsEnabled { get; set; }

    public int Width { get; set; } = 4;

    public int Height { get; set; } = 4;

    public float ToNextLaserCalc { get; set; }

    /// <summary>
    /// Gets or sets the time in seconds to the next laser hit.
    /// </summary>
    public float LaserTime { get; set; } = 0.05f;

    public List<Laser> CalculatedLaser { get; } = [];

    public List<GridItem> Items { get; } = [];

    public List<GridItem> NotPlacedItems { get; } = [];

    public event EventHandler? PropertyChanged;

    public void AddGridItem(GridItem item)
    {
      this.Items.Add(item);
      item.PlacedInGrid = true;
      this.NotPlacedItems.Remove(item);
      this.RecalculateLaser();
    }

    public void RemoveGridItem(GridItem item)
    {
      this.Items.Remove(item);
      item.PlacedInGrid = false;
      this.NotPlacedItems.Add(item);
      this.RecalculateLaser();
    }

    public void RecalculateLaser()
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
      var currentLaser = new Laser(currentDirection, currentPosition, currentPosition, 0, laserEmitter.LaserStrength);
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
              currentLaser = new Laser(currentDirection, nextPosition, nextPosition, 0, currentLaser.Strength);
              break;
            default:
              currentLaser = currentLaser with
              {
                To = currentPosition,
                HitTargetDistance = item == null ? 0.5f : 0.7f,
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

    public (bool IsFree, GridItem? Item) Check(Vector2 nextPosition)
    {
      if (nextPosition.X < 0 || nextPosition.X >= this.Width || nextPosition.Y < 0 || nextPosition.Y >= this.Height)
      {
        return (false, null);
      }

      var item = this.Items.FirstOrDefault(x => x.Position == nextPosition);
      return (item == null, item);
    }

    public void RaisePropertyChanged()
    {
      this.PropertyChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
