namespace IdleFactory.Observable
{
  public class BehaviorSubject<T> : ICustomObservable<T>
  {
    private IEqualityComparer<T>? comparer;
    public BehaviorSubject(bool compareValue = true)
    {
      if (compareValue)
      {
        this.comparer = EqualityComparer<T>.Default;
      }
    }

    public required T Value
    {
      get;
      set
      {
        if (this.comparer != null && this.comparer.Equals(field, value))
        {
          return;
        }

        field = value;
        this.ValueChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    public event EventHandler? ValueChanged;
  }
}
