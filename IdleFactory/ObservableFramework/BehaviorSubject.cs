namespace IdleFactory.Observable
{
  public class BehaviorSubject<T> : ICustomObservable<T>
  {
    public required T Value
    {
      get;
      set
      {
        field = value;
        this.ValueChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    public event EventHandler? ValueChanged;
  }
}
