namespace IdleFactory.Data
{
	public class FactoryData
	{
		public MainFactory MainFactory { get; } = new MainFactory();

		public EnergyGrid EnergyGrid { get; } = new EnergyGrid();

    public void AfterGameTick()
    {
      this.MainFactory.AfterGameTick();
    }
	}
}
