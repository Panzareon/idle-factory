namespace IdleFactory.Data.Energy
{
  public interface IEnergyGridBuff : IBuff
  {
    int AdjustLaserDistance(LaserEmitter laserEmitter, int baseValue);
  }
}
