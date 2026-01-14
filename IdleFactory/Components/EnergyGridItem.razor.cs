using IdleFactory.Data.Energy;
using Microsoft.AspNetCore.Components;

namespace IdleFactory.Components
{
  public partial class EnergyGridItem
  {
    [Parameter]
    public required GridItem Item { get; set; }

    private string GetFillPercent(UnpoweredItem unpoweredItem)
    {
      return ((double)(unpoweredItem.Power * 100 / unpoweredItem.RequiredPower)).ToString();
    }

    private string GetClassName()
    {
      var specificClassName = this.Item switch
      {
        LaserEmitter => "laser-emitter",
        Mirror => "mirror",
        UnpoweredItem => "unpowered-item",
        ProductionBuff => "production-buff",
        _ => "default"
      };

      return $"grid-item-{specificClassName}";
    }
  }
}