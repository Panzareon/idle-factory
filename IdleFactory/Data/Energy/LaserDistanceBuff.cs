namespace IdleFactory.Data.Energy
{
  public class LaserDistanceBuff : GridItem, IEnergyGridBuff
  {
    public int Order { get; } = 0;

    public int AdjustLaserDistance(LaserEmitter laserEmitter, int baseValue)
    {
      var deltaX = laserEmitter.Position.X - this.Position.X;
      var deltaY = laserEmitter.Position.Y - this.Position.Y;
      var squareDistance = deltaX * deltaX + deltaY * deltaY;
      if (squareDistance != 1)
      {
        return baseValue;
      }


      return baseValue + 2;
    }
  }
}
