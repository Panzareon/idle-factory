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

    private Vector2? currentDragOver;

    private GridItem? currentDraggingItem;

    [Inject]
    public required FactoryDataService FactoryDataService { get; set; }

    public GridItem? SelectedItem { get; set; }

    private EnergyGrid EnergyGrid => this.FactoryDataService.Data.EnergyGrid;

    public void Dispose()
    {
      this.EnergyGrid.PropertyChanged -= this.EnergyGridPropertyChanged;
      this.EnergyGrid.Items.CollectionChanged -= this.EnergyGridCollectionChanged;
      this.EnergyGrid.BuildableItems.CollectionChanged -= this.EnergyGridBuildableCollectionChanged;
      this.EnergyGrid.NotPlacedItems.CollectionChanged -= this.EnergyGridCollectionChanged;
    }

    protected override void OnInitialized()
    {
      base.OnInitialized();
      this.EnergyGrid.PropertyChanged += this.EnergyGridPropertyChanged;
      this.EnergyGrid.Items.CollectionChanged += this.EnergyGridCollectionChanged;
      this.EnergyGrid.BuildableItems.CollectionChanged += this.EnergyGridBuildableCollectionChanged;
      this.EnergyGrid.NotPlacedItems.CollectionChanged += this.EnergyGridCollectionChanged;
    }

    private void EnergyGridPropertyChanged(object? sender, EventArgs e)
    {
      this.StateHasChanged();
    }

    private void EnergyGridBuildableCollectionChanged(object? sender, ICollectionChangedEventArgs<BuildableItem> e)
    {
      this.StateHasChanged();
    }

    private void EnergyGridCollectionChanged(object? sender, ICollectionChangedEventArgs<GridItem> e)
    {
      this.StateHasChanged();
    }

    private void StartDrag(GridItem dragginItem)
    {
      this.currentDraggingItem = dragginItem;
    }

    private void Drop(Vector2 position)
    {
      if (this.currentDraggingItem != null)
      {
        this.MoveToGrid(position, this.currentDraggingItem);
      }
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
          this.EnergyGrid.NeedRecalculateLaser();
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

          this.EnergyGrid.NeedRecalculateLaser();
          break;
      }
    }

    private void SelectGridPosition(Vector2 position)
    {
      if (this.SelectedItem == null)
      {
        return;
      }

      if (this.MoveToGrid(position, this.SelectedItem))
      {
        this.SelectedItem = null;
      }
    }

    private bool MoveToGrid(Vector2 position, GridItem selectedItem)
    {
      if (this.EnergyGrid.Check(position).IsFree)
      {
        selectedItem.Position = position;
        if (selectedItem.PlacedInGrid)
        {
          this.EnergyGrid.NeedRecalculateLaser();
        }
        else
        {
          var buildableItem = this.EnergyGrid.BuildableItems.FirstOrDefault(x => x.PreviewItem == selectedItem);
          if (buildableItem == null)
          {
            this.EnergyGrid.AddGridItem(selectedItem);
          }
          else
          {
            var unpoweredItem = buildableItem.BuildItem(this.EnergyGrid);
            unpoweredItem.Position = position;
            this.EnergyGrid.AddGridItem(unpoweredItem);
          }
        }

        return true;
      }

      return false;
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

    private void DropNotPlacedItems()
    {
      if (this.currentDraggingItem == null || !this.currentDraggingItem.PlacedInGrid)
      {
        return;
      }

      this.EnergyGrid.RemoveGridItem(this.currentDraggingItem);
    }
  }
}