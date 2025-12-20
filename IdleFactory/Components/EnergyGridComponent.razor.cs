using IdleFactory.Data.Energy;
using IdleFactory.Services;
using Microsoft.AspNetCore.Components;

namespace IdleFactory.Components
{
  public partial class EnergyGridComponent
  {
    /// <summary>
    /// Size of the individual grid cells in pixel.
    /// </summary>
    private const int GridSize = 30;

    [Inject]
    public required FactoryDataService FactoryDataService { get; set; }

    private EnergyGrid EnergyGrid => this.FactoryDataService.Data.EnergyGrid;

    private string GetClassName(GridItem gridItem)
    {
      var specificClassName = gridItem switch
      {
        LaserEmitter => "laser-emitter",
        Mirror => "mirror",
        _ => "default"
      };

      return $"grid-item-{specificClassName}";
    }
    private static float GetLength(Laser laser)
    {
      var baseLength = MathF.Abs(laser.From.X == laser.To.X ? laser.From.Y - laser.To.Y : laser.From.X - laser.To.X);

      if (laser.HitTarget)
      {
        return baseLength + 0.5f;
      }

      return baseLength;
    }

    private static float GetStartPositionX(Laser laser)
    {
      if (laser.Direction.X == 0)
      {
        return laser.From.X + 0.5f;
      }

      if (laser.Direction.X < 0)
      {
        if (laser.HitTarget)
        {
          return laser.To.X;
        }

        return laser.To.X + 0.5f;
      }

      return laser.From.X + 0.5f;
    }

    private static float GetStartPositionY(Laser laser)
    {
      if (laser.Direction.Y == 0)
      {
        return laser.From.Y + 0.5f;
      }

      if (laser.Direction.Y < 0)
      {
        if (laser.HitTarget)
        {
          return laser.To.Y;
        }

        return laser.To.Y + 0.5f;
      }

      return laser.From.Y + 0.5f;
    }
  }
}