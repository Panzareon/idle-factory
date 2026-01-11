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

    public void Dispose()
    {
      this.EnergyGrid.PropertyChanged -= this.EnergyGridPropertyChanged;
    }

    protected override void OnInitialized()
    {
      base.OnInitialized();
      this.EnergyGrid.PropertyChanged += this.EnergyGridPropertyChanged;
    }

    private void EnergyGridPropertyChanged(object? sender, EventArgs e)
    {
      this.StateHasChanged();
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
      if (this.SelectedItem?.PlacedInGrid == true && !gridItem.PlacedInGrid)
      {
        this.SelectNotPlacedItems();
        return;
      }

      if ((e.ShiftKey || e.Detail >= 2) && this.EnergyGrid.BuildableItems.FirstOrDefault(x => x.PreviewItem == gridItem) == null)
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
        if (this.SelectedItem.PlacedInGrid)
        {
          this.EnergyGrid.RecalculateLaser();
        }
        else
        {
          var buildableItem = this.EnergyGrid.BuildableItems.FirstOrDefault(x => x.PreviewItem == this.SelectedItem);
          if (buildableItem == null)
          {
            this.EnergyGrid.AddGridItem(this.SelectedItem);
          }
          else
          {
            var unpoweredItem = buildableItem.BuildItem();
            unpoweredItem.Position = position;
            this.EnergyGrid.AddGridItem(unpoweredItem);
          }
        }
        
        this.SelectedItem = null;
      }
    }

    private void SelectNotPlacedItems()
    {
      if (this.SelectedItem == null || !this.SelectedItem.PlacedInGrid)
      {
        return;
      }

      this.EnergyGrid.RemoveGridItem(this.SelectedItem);
      this.SelectedItem = null;
    }
  }
}