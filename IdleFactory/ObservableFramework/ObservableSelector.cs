namespace IdleFactory.Observable
{
  internal class ObservableSelector<TSource, TTarget>(ICustomObservable<TSource> source, Func<TSource, TTarget> selector) : DerivedObservable<TTarget>(source)
  {
    protected override TTarget CalculateValue(bool attached)
    {
      return selector(source.Value);
    }
  }
}