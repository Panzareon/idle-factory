using IdleFactory.Data.Energy;
using IdleFactory.Data.Main;

namespace IdleFactory.Data
{
	public class FactoryData
	{
		public MainFactory MainFactory { get; } = new MainFactory();

		public EnergyGrid EnergyGrid { get; } = new EnergyGrid();

    public IEnumerable<IBuff> Buffs => this.EnergyGrid.Buffs;

    public void AfterGameTick()
    {
      this.MainFactory.AfterGameTick();
    }
	}
}
