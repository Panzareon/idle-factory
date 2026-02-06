namespace IdleFactory.Observable
{
  public class FixedValue<T> : ICustomObservable<T>
  {
    public required T Value { get; init; } 

    public event EventHandler ValueChanged
    {
      add { }
      remove { }
    }
  }
}
