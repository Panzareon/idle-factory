using IdleFactory.Data;
using IdleFactory.Data.Energy;
using IdleFactory.Data.Main;
using System.Diagnostics;

namespace IdleFactory.Services
{
  public sealed class GameLogicService(FactoryDataService factoryDataService) : IDisposable
  {
    private static readonly TimeSpan targetTickTime = TimeSpan.FromSeconds(1 / 20f);
    private Timer timer;
    private Stopwatch stopwatch = new Stopwatch();

    public void Init()
    {
      if (this.timer != null)
      {
        // already initialized
        return;
      }

      factoryDataService.Data.MainFactory.ResourceGenerators.Add(new ResourceGenerator { ResourceType = ResourceType.Red, GenerationAmount = 1, GenerationTime = 1 });
      factoryDataService.Data.MainFactory.Resources.Add(ResourceType.Red, 0);
      factoryDataService.Data.EnergyGrid.AddGridItem(new LaserEmitter { Direction = new Vector2(0, 1), Position = new Vector2(0, 0), LaserStrength = 1 });
      factoryDataService.Data.EnergyGrid.BuildableItems.Add(new BuildableItem(BuildableItemType.LaserEmitter));
      factoryDataService.Data.EnergyGrid.BuildableItems.Add(new BuildableItem(BuildableItemType.Mirror));
      this.stopwatch.Start();
      this.timer = new Timer(this.OnTimerTick, null, TimeSpan.Zero, targetTickTime);
    }

    private void OnTimerTick(object? state)
    {
      var deltaTime = (float)this.stopwatch.Elapsed.TotalSeconds;
      this.stopwatch.Restart();
      this.GameTick(deltaTime);
    }

    /// <summary>
    /// Handles the game tick.
    /// </summary>
    /// <param name="deltaTime">The delta time since the last tick.</param>
    public void GameTick(float deltaTime)
    {
      this.GameTick(factoryDataService.Data.MainFactory, deltaTime);
      this.GameTick(factoryDataService.Data.EnergyGrid, deltaTime);

      factoryDataService.Data.AfterGameTick();
    }

    /// <summary>
    /// Handles the game tick for the main factory.
    /// </summary>
    /// <param name="mainFactory">The main factory to handle.</param>
    /// <param name="deltaTime">The delta time since the last tick.</param>
    private void GameTick(MainFactory mainFactory, float deltaTime)
    {
      foreach (var generator in mainFactory.ResourceGenerators)
      {
        if (!generator.IsEnabled)
        {
          continue;
        }

        var multiplier = 1.0f;
        if (generator.IsFocused)
        {
          multiplier *= 2.0f;
        }

        generator.ToNextGeneration += deltaTime * multiplier;
        if (generator.ToNextGeneration > generator.GenerationTime)
        {
          var numberOfTicks = (ulong)(generator.ToNextGeneration / generator.GenerationTime);
          generator.ToNextGeneration -= numberOfTicks * generator.GenerationTime;
          var amount = generator.GenerationAmount * numberOfTicks;
          if (generator.ConvertFrom != ResourceType.Undefined)
          {
            var conversionCost = amount / generator.ConversionFactor;
            if (mainFactory.HasResource(generator.ConvertFrom, conversionCost))
            {
              mainFactory.Remove(new ResourceCost(generator.ConvertFrom, conversionCost));
            }
            else
            {
              var availableAmount = mainFactory.GetResource(generator.ConvertFrom);
              amount = availableAmount * generator.ConversionFactor;
              var amountToRemove = amount / generator.ConversionFactor;
              if (amountToRemove > availableAmount)
              {
                amount -= 1;
                amountToRemove = amount / generator.ConversionFactor;
              }

              mainFactory.Remove(new ResourceCost(generator.ConvertFrom, amountToRemove));
            }
          }

          mainFactory.Add(generator.ResourceType, amount);
        }
      }
    }

    private void GameTick(EnergyGrid energyGrid, float deltaTime)
    {
      energyGrid.ToNextLaserCalc += deltaTime;
      if (energyGrid.ToNextLaserCalc < energyGrid.LaserTime)
      {
        return;
      }

      var hasChanged = false;
      var numberOfLasers = MathF.Floor(energyGrid.ToNextLaserCalc / energyGrid.LaserTime);
      energyGrid.ToNextLaserCalc -= numberOfLasers * energyGrid.LaserTime;
      foreach (var laser in energyGrid.CalculatedLaser)
      {
        if (laser.HitTargetDistance == 0)
        {
          // Nothing was hit
          continue;
        }

        var targetPosition = laser.To + laser.Direction;
        var target = energyGrid.Items.FirstOrDefault(x => x.Position == targetPosition);
        if (target == null)
        {
          // Hit the wall instead of an item
          continue;
        }

        if (target is IPowerSinkGridItem powerSink)
        {
          powerSink.HitByLaser(laser.Strength, numberOfLasers);
          hasChanged = true;
        }
      }

      foreach (var gridItem in energyGrid.Items.ToList())
      {
        if (gridItem is UnpoweredItem unpoweredItem && unpoweredItem.Power >= unpoweredItem.RequiredPower)
        {
          // Remove without adding to the not placed items
          energyGrid.Items.Remove(unpoweredItem);

          unpoweredItem.BuildTarget.Position = unpoweredItem.Position;
          energyGrid.AddGridItem(unpoweredItem.BuildTarget);
          hasChanged = true;
        }
      }

      if (hasChanged)
      {
        energyGrid.RaisePropertyChanged();
      }
    }

    public void Dispose()
    {
      this.timer?.Dispose();
    }
  }
}
