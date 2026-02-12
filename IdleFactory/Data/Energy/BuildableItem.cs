
using IdleFactory.Data.Main;

namespace IdleFactory.Data.Energy
{
  public enum BuildableItemType
  {
    LaserEmitter = 1,
    Mirror = 2,
    RedProductionBuff = 3,
    LaserDistanceBuff = 4,
  }

  public class BuildableItem
  {
    public BuildableItem(BuildableItemType type)
    {
      this.Type = type;
      this.PreviewItem = this.CreateItem();
    }

    public BuildableItemType Type { get; }

    public GridItem PreviewItem { get; }

    /// <summary>
    /// Gets or sets how often this buildable item can be built.
    /// Negative values means it can be build indefinetely.
    /// </summary>
    public int NumberOfItemsAvailable { get; set; } = 1;

    public UnpoweredItem BuildItem(EnergyGrid energyGrid)
    {
      if (this.NumberOfItemsAvailable == 0)
      {
        throw new InvalidOperationException("Cannot build this item anymore");
      }

      if (this.NumberOfItemsAvailable > 0)
      {
        this.NumberOfItemsAvailable--;
        if (this.NumberOfItemsAvailable == 0)
        {
          energyGrid.BuildableItems.Remove(this);
        }
      }

      return new UnpoweredItem
      {
        BuildableItem = this,
        BuildTarget = this.CreateItem(),
        RequiredPower = this.GetRequiredPower(),
      };
    }

    private LargeInteger GetRequiredPower()
    {
      switch (this.Type)
      {
        case BuildableItemType.LaserEmitter:
          return 100;
        case BuildableItemType.Mirror:
          return 200;
        case BuildableItemType.RedProductionBuff:
          return 400;
        case BuildableItemType.LaserDistanceBuff:
          return 400;
        default:
          throw new InvalidOperationException($"Cannot create item of type {this.Type}");
      }
    }

    private GridItem CreateItem()
    {
      switch (this.Type)
      {
        case BuildableItemType.LaserEmitter:
          return new LaserEmitter
          {
            LaserStrength = 1,
            Direction = new Vector2(1, 0),
          };
        case BuildableItemType.Mirror:
          return new Mirror();
        case BuildableItemType.RedProductionBuff:
          return new ProductionBuff(ResourceType.Red);
        case BuildableItemType.LaserDistanceBuff:
          return new LaserDistanceBuff();
        default:
          throw new InvalidOperationException($"Cannot create item of type {this.Type}");
      }
    }
  }
}
