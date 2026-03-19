using IdleFactory.Data.Energy;
using Microsoft.AspNetCore.Components;

namespace IdleFactory.Components
{
  public sealed partial class EnergyGridItem : IDisposable
  {
    private Observer? gridItemObserver;

    [Parameter]
    public required GridItem Item { get; set; }

    protected override void OnInitialized()
    {
      base.OnInitialized();
      this.gridItemObserver = this.Item.Observables.AsObservableCollection().OnAnyChanged();
      this.gridItemObserver.ValueChanged += this.GridItemChanged;
    }

    private void GridItemChanged(object? sender, EventArgs e)
    {
      this.StateHasChanged();
    }

    private string GetFillPercent(UnpoweredItem unpoweredItem)
    {
      return ((double)(unpoweredItem.Power.Value * 100 / unpoweredItem.RequiredPower)).ToString();
    }

    private string GetClassName()
    {
      var specificClassName = this.Item switch
      {
        LaserEmitter => "laser-emitter",
        Mirror => "mirror",
        UnpoweredItem => "unpowered-item",
        ProductionBuff => "production-buff",
        ProductionEfficiencyBuff => "production-efficiency-buff",
        LaserDistanceBuff => "laser-distance-buff",
        LaserRelayGridItem => "laser-relay",
        _ => "default"
      };

      return $"grid-item-{specificClassName}";
    }

    public void Dispose()
    {
      this.gridItemObserver?.ValueChanged -= this.GridItemChanged;
    }
  }
}