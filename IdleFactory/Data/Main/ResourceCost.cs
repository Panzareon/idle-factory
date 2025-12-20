namespace IdleFactory.Data.Main
{
  public record ResourceCost(ResourceType ResourceType, LargeInteger Amount);

  public static class ResourceCostExtensions
  {
    extension(IEnumerable<ResourceCost> resourceCosts)
    {
      public string ToCostString()
      {
        return string.Join(" + ", resourceCosts.Select(x => $"{x.Amount.ToString(Constants.DefaultDisplayPrecision)} {x.ResourceType}"));
      }
    }
  }
}
