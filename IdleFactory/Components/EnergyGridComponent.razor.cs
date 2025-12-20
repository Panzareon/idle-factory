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
        _ => "default"
      };

      return $"grid-item-{specificClassName}";
    }
  }
}