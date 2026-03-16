namespace IdleFactory.Data.Energy
{
  public record Vector2(int X, int Y)
  {
    public static Vector2 operator +(Vector2 left, Vector2 right)
    {
      return new Vector2(left.X + right.X, left.Y + right.Y);
    }
  }

  public static class Vector2Extensions
  {
    public static IEnumerable<Vector2> GetAdjacent(this Vector2 position)
    {
      yield return new Vector2(position.X + 1, position.Y);
      yield return new Vector2(position.X, position.Y + 1);
      yield return new Vector2(position.X - 1, position.Y);
      yield return new Vector2(position.X, position.Y - 1);
    }
  }
}
