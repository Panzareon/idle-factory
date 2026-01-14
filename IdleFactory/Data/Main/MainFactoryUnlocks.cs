using IdleFactory.Data.Energy;

namespace IdleFactory.Data.Main
{
  public enum MainFactoryUnlocks
  {
    Undefined = 0,

    FocusGenerator = 1,

    RedGenerator2 = 2,

    RedToBlue = 3,

    EnergyGrid = 4,

    RedProductionBuffItem = 5,
  }

  public static class MainFactoryUnlocksExtensions
  {
    extension(MainFactoryUnlocks unlock)
    {
      public IReadOnlyList<ResourceCost> GetCosts()
      {
        switch (unlock)
        {
          case MainFactoryUnlocks.FocusGenerator:
            return [new ResourceCost(ResourceType.Red, 1000)];
          case MainFactoryUnlocks.RedGenerator2:
            return [new ResourceCost(ResourceType.Red, 500)];
          case MainFactoryUnlocks.RedToBlue:
            return [new ResourceCost(ResourceType.Red, 20000)];
          case MainFactoryUnlocks.EnergyGrid:
            return [new ResourceCost(ResourceType.Red, 20000), new ResourceCost(ResourceType.Blue, 100)];
          case MainFactoryUnlocks.RedProductionBuffItem:
            return [new ResourceCost(ResourceType.Red, 50000), new ResourceCost(ResourceType.Blue, 1000)];
          default:
            throw new InvalidOperationException($"No costs specified for unlock {unlock}");
        }
      }

      public void Apply(MainFactory mainFactory, FactoryData data)
      {
        switch (unlock)
        {
          case MainFactoryUnlocks.RedGenerator2:
            mainFactory.ResourceGenerators.Add(new ResourceGenerator { GenerationAmount = 10, GenerationTime = 1, ResourceType = ResourceType.Red });
            break;
          case MainFactoryUnlocks.RedToBlue:
            mainFactory.ResourceGenerators.Add(new ResourceGenerator { GenerationAmount = 1, GenerationTime = 1, ResourceType = ResourceType.Blue, ConvertFrom = ResourceType.Red, ConversionFactor = 0.001f });
            break;
          case MainFactoryUnlocks.EnergyGrid:
            data.EnergyGrid.IsEnabled = true;
            break;
          case MainFactoryUnlocks.RedProductionBuffItem:
            data.EnergyGrid.AddBuildableItem(BuildableItemType.RedProductionBuff);
            break;
        }
      }
    }
  }
}
