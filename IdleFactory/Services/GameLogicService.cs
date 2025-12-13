using IdleFactory.Data;

namespace IdleFactory.Services
{
  public sealed class GameLogicService(FactoryDataService factoryDataService) : IDisposable
  {
    private static readonly TimeSpan targetTickTime = TimeSpan.FromSeconds(1 / 60f);
    private Timer timer;

    public void Init()
    {
      if (this.timer != null)
      {
        // already initialized
        return;
      }

      factoryDataService.Data.MainFactory.ResourceGenerators.Add(new ResourceGenerator { ResourceType = ResourceType.Red, GenerationAmount = 1, GenerationTime = 1 });
      factoryDataService.Data.MainFactory.Resources.Add(ResourceType.Red, 0);
      this.timer = new Timer(this.OnTimerTick, null, TimeSpan.Zero, targetTickTime);
    }

    private void OnTimerTick(object? state)
    {
      this.GameTick((float)targetTickTime.TotalSeconds);
    }

    /// <summary>
    /// Handles the game tick.
    /// </summary>
    /// <param name="deltaTime">The delta time since the last tick.</param>
    public void GameTick(float deltaTime)
    {
      this.GameTick(factoryDataService.Data.MainFactory, deltaTime);
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
          mainFactory.Add(generator.ResourceType, generator.GenerationAmount * numberOfTicks);
        }
      }
    }

    public void Dispose()
    {
      this.timer?.Dispose();
    }
  }
}
