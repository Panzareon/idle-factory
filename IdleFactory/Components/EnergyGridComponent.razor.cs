using IdleFactory.Data.Energy;
using IdleFactory.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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

    public GridItem? SelectedItem { get; set; }

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

      return baseLength + laser.HitTargetDistance;
    }

    private static float GetStartPositionX(Laser laser)
    {
      if (laser.Direction.X == 0)
      {
        return laser.From.X + 0.5f;
      }

      if (laser.Direction.X < 0)
      {
        return laser.To.X + 0.5f - laser.HitTargetDistance;
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
        return laser.To.Y + 0.5f - laser.HitTargetDistance;
      }

      return laser.From.Y + 0.5f;
    }

    private void SelectGridItem(MouseEventArgs e, GridItem gridItem)
    {
      if (e.ShiftKey || e.Detail >= 2)
      {
        this.Rotate(gridItem);
        return;
      }

      if (this.SelectedItem == gridItem)
      {
        this.SelectedItem = null;
      }
      else
      {
        this.SelectedItem = gridItem;
      }
    }

    private void Rotate(GridItem selectedItem)
    {
      switch (selectedItem)
      {
        case Mirror mirror:
          mirror.PositiveDirection = !mirror.PositiveDirection;
          this.EnergyGrid.RecalculateLaser();
          break;
        case LaserEmitter laserEmitter:
          if (laserEmitter.Direction.X > 0)
          {
            laserEmitter.Direction = new Vector2(0, 1);
          }
          else if (laserEmitter.Direction.Y > 0)
          {
            laserEmitter.Direction = new Vector2(-1, 0);
          }
          else if (laserEmitter.Direction.X < 0)
          {
            laserEmitter.Direction = new Vector2(0, -1);
          }
          else
          {
            laserEmitter.Direction = new Vector2(1, 0);
          }

          this.EnergyGrid.RecalculateLaser();
          break;
      }
    }

    private void SelectGrid(MouseEventArgs e)
    {
      if (this.SelectedItem == null)
      {
        return;
      }

      var position = new Vector2((int)(e.OffsetX / GridSize), (int)(e.OffsetY / GridSize));
      if (this.EnergyGrid.Check(position).IsFree)
      {
        this.SelectedItem.Position = position;
        this.EnergyGrid.RecalculateLaser();
        this.SelectedItem = null;
      }
    }
  }
}