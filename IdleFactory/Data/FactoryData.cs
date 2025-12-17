namespace IdleFactory.Data
{
	public class FactoryData
	{
		public MainFactory MainFactory { get; } = new MainFactory();

    public void AfterGameTick()
    {
      this.MainFactory.AfterGameTick();
    }
	}
}
