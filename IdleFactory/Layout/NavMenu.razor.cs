using IdleFactory.Data.Energy;
using IdleFactory.Services;

namespace IdleFactory.Layout
{
  public partial class NavMenu(FactoryDataService factoryDataService) : IDisposable
  {
    private bool collapseNavMenu = true;

    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    private EnergyGrid EnergyGrid => factoryDataService.Data.EnergyGrid;

    protected override void OnInitialized()
    {
      base.OnInitialized();
      if (!this.EnergyGrid.IsEnabled)
      {
        this.EnergyGrid.IsEnabledObservable.ValueChanged += ValueChanged;
      }
    }

    private void ValueChanged(object? sender, EventArgs e)
    {
      this.StateHasChanged();
    }

    public void Dispose()
    {
      this.EnergyGrid.IsEnabledObservable.ValueChanged -= ValueChanged;
    }

    private void ToggleNavMenu()
    {
      collapseNavMenu = !collapseNavMenu;
    }
  }
}