using IdleFactory.Data.Energy;
using Microsoft.AspNetCore.Components;

namespace IdleFactory.Components
{
  public partial class EnergyGridItem
  {
    [Parameter]
    public required GridItem Item { get; set; }

    private string GetClassName()
    {
      var specificClassName = this.Item switch
      {
        LaserEmitter => "laser-emitter",
        Mirror => "mirror",
        _ => "default"
      };

      return $"grid-item-{specificClassName}";
    }
  }
}