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

    private bool IsFocusAvailable => this.MainFactory.Unlocks.Contains(MainFactoryUnlocks.FocusGenerator);

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

    private void ToggleFocus(ResourceGenerator resourceGenerator)
    {
      if (resourceGenerator.IsFocused)
      {
        resourceGenerator.IsFocused = false;
      }
      else
      {
        resourceGenerator.IsFocused = true;
        foreach (var otherGenerator in this.MainFactory.ResourceGenerators)
        {
          if (otherGenerator != resourceGenerator)
          {
            otherGenerator.IsFocused = false;
          }
        }
      }
    }

    private bool CanUpgrade(ResourceGenerator resourceGenerator)
    {
      var upgradeCost = resourceGenerator.GetUpgradeCost().ToList();
      return this.MainFactory.HasResources(upgradeCost);
    }

    private void Upgrade(ResourceGenerator resourceGenerator)
    {
      var upgradeCost = resourceGenerator.GetUpgradeCost().ToList();
      if (!this.MainFactory.HasResources(upgradeCost))
      {
        return;
      }

      this.MainFactory.Remove(upgradeCost);
      resourceGenerator.Upgrade();
    }

    private void Unlock(MainFactoryUnlocks unlock)
    {
      var costs = unlock.GetCosts();
      if (!this.MainFactory.HasResources(costs))
      {
        return;
      }

      this.MainFactory.Remove(costs);
      this.MainFactory.Unlocks.Add(unlock);
    }

    private IEnumerable<(MainFactoryUnlocks Unlock, string CostString, bool CanBuy)> GetVisibleUnlocks()
    {
      if (!this.MainFactory.Unlocks.Contains(MainFactoryUnlocks.FocusGenerator))
      {
        var costs = MainFactoryUnlocks.FocusGenerator.GetCosts();
        yield return (MainFactoryUnlocks.FocusGenerator, costs.ToCostString(), this.MainFactory.HasResources(costs));
      }
    }
  }
}