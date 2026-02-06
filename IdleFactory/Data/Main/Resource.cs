
namespace IdleFactory.Data.Main
{
  public class Resource
  {
    private readonly BehaviorSubject<LargeInteger> currentAmount = new() { Value = 0 };

    private bool hasChanged = false;

    public event EventHandler? PropertyChanged;

    public LargeInteger Amount
    {
      get => this.currentAmount.Value;
      set
      {
        this.currentAmount.Value = value;
        this.hasChanged = true;
      }
    }

    public ICustomObservable<LargeInteger> ObservableAmount => this.currentAmount;

    public void AfterGameTick()
    {
      if (!this.hasChanged)
      {
        return;
      }

      this.PropertyChanged?.Invoke(this, EventArgs.Empty);
      this.hasChanged = false;
    }
  }
}
