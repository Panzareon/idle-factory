namespace IdleFactory.Data
{
  public enum MainFactoryUnlocks
  {
    Undefined = 0,

    FocusGenerator = 1,

    RedGenerator2 = 2,
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
          default:
            throw new InvalidOperationException($"No costs specified for unlock {unlock}");
        }
      }

      public void Apply(MainFactory mainFactory)
      {
        switch (unlock)
        {
          case MainFactoryUnlocks.RedGenerator2:
            mainFactory.ResourceGenerators.Add(new ResourceGenerator { GenerationAmount = 10, GenerationTime = 1, ResourceType = ResourceType.Red });
            break;
        }
      }
    }
  }
}
