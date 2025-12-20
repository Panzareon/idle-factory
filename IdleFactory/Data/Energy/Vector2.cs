namespace IdleFactory.Data.Energy
{
  public record Vector2(int X, int Y)
  {
    public static Vector2 operator +(Vector2 left, Vector2 right)
    {
      return new Vector2(left.X + right.X, left.Y + right.Y);
    }
  }
}
