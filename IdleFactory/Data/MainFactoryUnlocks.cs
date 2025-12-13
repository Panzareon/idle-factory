namespace IdleFactory.Data
{
  public enum MainFactoryUnlocks
  {
    Undefined = 0,

    FocusGenerator = 1,
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
          default:
            throw new InvalidOperationException($"No costs specified for unlock {unlock}");
        }
      }
    }
  }
}
