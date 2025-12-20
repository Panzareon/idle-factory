namespace IdleFactory.Data.Energy
{
  public record Laser(Vector2 From, Vector2 To, bool HitTarget, LargeInteger Strength)
  {
    public float GetLength()
    {
      var baseLength = MathF.Abs(this.From.X == this.To.X ? this.From.Y - this.To.Y : this.From.X - this.To.X);

      if (HitTarget)
      {
        return baseLength + 0.5f;
      }

      return baseLength;
    }
  }
}
