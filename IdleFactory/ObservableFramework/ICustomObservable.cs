namespace IdleFactory.Observable
{
  public interface ICustomObservable
  {
    event EventHandler ValueChanged;
  }

  public interface ICustomObservable<T> : ICustomObservable
  {
    T Value { get; }
  }
}
