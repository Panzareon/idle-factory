namespace IdleFactory.Data.Energy
{
  public class LaserEmitter : GridItem
  {
    public required Vector2 Direction { get; set; }

    public required LargeInteger LaserStrength { get; set; }

    public int MaxDistance { get; set; } = 2;
  }
}
