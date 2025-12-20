using IdleFactory.Data.Energy;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdleFactory.Tests.Data
{
  internal class EnergyGridTests
  {
    [Test]
    public void CalculateLaserTest()
    {
      var grid = new EnergyGrid
      {
        Width = 4,
        Height = 4,
      };
      var emitter = new LaserEmitter { Position = new Vector2(0, 0), Direction = new Vector2(0, 1), MaxDistance = 2 };
      grid.AddGridItem(emitter);

      Assert.That(grid.CalculatedLaser, Is.EqualTo([new Laser(new Vector2(0, 0), new Vector2(0, 2), false, emitter.LaserStrength)]));
    }

    [Test]
    public void CalculateLaserHitWallTest()
    {
      var grid = new EnergyGrid
      {
        Width = 4,
        Height = 4,
      };
      var emitter = new LaserEmitter { Position = new Vector2(0, 0), Direction = new Vector2(0, 1), MaxDistance = 4 };
      grid.AddGridItem(emitter);

      Assert.That(grid.CalculatedLaser, Is.EqualTo([new Laser(new Vector2(0, 0), new Vector2(0, 3), true, emitter.LaserStrength)]));
    }
  }
}
