using IdleFactory.Data;
using IdleFactory.Services;
using Microsoft.AspNetCore.Components;

namespace IdleFactory.Components
{
  public partial class MainFactoryComponent : IDisposable
  {
    [Inject]
    public FactoryDataService FactoryDataService { get; set; }

    public MainFactory MainFactory => this.FactoryDataService.Data.MainFactory;

    public void Dispose()
    {
      this.MainFactory.PropertyChanged -= MainFactoryPropertyChanged;
    }

    protected override void OnInitialized()
    {
      base.OnInitialized();
      this.MainFactory.PropertyChanged += this.MainFactoryPropertyChanged;
    }

    private void MainFactoryPropertyChanged(object? sender, EventArgs e)
    {
      this.StateHasChanged();
    }
  }
}