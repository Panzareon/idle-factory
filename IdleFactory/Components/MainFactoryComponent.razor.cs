using IdleFactory.Data.Main;
using IdleFactory.ObservableFramework;
using IdleFactory.Services;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace IdleFactory.Components
{
  public partial class MainFactoryComponent : IDisposable
  {
    private IReadOnlyList<AvailableUnlock> availableUnlocks = [];
    private Observer? observer;

    [Inject]
    public required FactoryDataService FactoryDataService { get; set; }

    public MainFactory MainFactory => this.FactoryDataService.Data.MainFactory;

    private bool IsFocusAvailable => this.MainFactory.Unlocks.Contains(MainFactoryUnlocks.FocusGenerator);

    private bool AllowSkip
    {
      get
      {
#if DEBUG
        return true;
#else
        return false;
#endif
      }
    }
    private void Skip()
    {
      MainFactory.Add(ResourceType.Red, 2000);
      MainFactory.Add(ResourceType.Blue, 1000);
    }

    public void Dispose()
    {
      this.MainFactory.PropertyChanged -= RelevantValueChanged;
      this.observer?.ValueChanged -= RelevantValueChanged;
    }

    protected override void OnInitialized()
    {
      base.OnInitialized();
      this.MainFactory.PropertyChanged += this.RelevantValueChanged;
      this.availableUnlocks =
        availableUnlocks = this.GetAvailableUnlockTypes()
        .Select(availableUnlock =>
        {
          var costs = availableUnlock.UnlockType.GetCosts();
          return new AvailableUnlock(availableUnlock.UnlockType, costs.ToCostString(), this.MainFactory.HasResourcesObservable(costs), availableUnlock.IsAvailable, this.MainFactory.Unlocks.Contains(availableUnlock.UnlockType));
        }).ToList();
      this.observer = this.availableUnlocks.SelectMany(x => x.Observables).OnAnyChanged();
      this.observer.ValueChanged += this.RelevantValueChanged;
    }

    private void RelevantValueChanged(object? sender, EventArgs e)
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

    private void Unlock(AvailableUnlock availableUnlock)
    {
      var unlock = availableUnlock.Unlock;
      var costs = unlock.GetCosts();
      if (!this.MainFactory.HasResources(costs))
      {
        return;
      }

      this.MainFactory.Remove(costs);
      this.MainFactory.Unlocks.Add(unlock);
      unlock.Apply(this.MainFactory, this.FactoryDataService.Data);
      availableUnlock.HasUnlocked();
    }

    private IEnumerable<AvailableUnlock> GetVisibleUnlocks()
    {
      return availableUnlocks.Where(x => x.IsVisible);
    }

    private IEnumerable<(MainFactoryUnlocks UnlockType, ICustomObservable<bool> IsAvailable)> GetAvailableUnlockTypes()
    {
      yield return (MainFactoryUnlocks.RedGenerator2, CustomObservable.Return(true));
      yield return (MainFactoryUnlocks.FocusGenerator, CustomObservable.Return(true));
      yield return (MainFactoryUnlocks.RedToBlue, this.MainFactory.HasResourceObservable(ResourceType.Red, 1200));
      yield return (MainFactoryUnlocks.EnergyGrid, this.MainFactory.HasResourceObservable(ResourceType.Red, 5000));
      yield return (MainFactoryUnlocks.RedProductionBuffItem, this.FactoryDataService.Data.EnergyGrid.IsEnabledObservable);
      yield return (MainFactoryUnlocks.LaserDistanceBuffItem, this.FactoryDataService.Data.EnergyGrid.IsEnabledObservable);
    }

    private class AvailableUnlock(MainFactoryUnlocks unlock, string costString, ICustomObservable<bool> canBuy, ICustomObservable<bool> isAvailable, bool alreadyUnlocked)
    {
      private bool wasVisible = false;
      private bool hasUnlocked = alreadyUnlocked;

      public MainFactoryUnlocks Unlock { get; } = unlock;

      public string CostString { get; } = costString;

      public bool CanBuy => canBuy.Value;

      public bool IsVisible
      {
        get
        {
          if (!isAvailable.Value && !this.wasVisible)
          {
            return false;
          }

          this.wasVisible = true;
          return !hasUnlocked;
        }
      }

      public void HasUnlocked()
      {
        this.hasUnlocked = true;
      }

      public IEnumerable<ICustomObservable> Observables => [canBuy, isAvailable];
    }
  }
}