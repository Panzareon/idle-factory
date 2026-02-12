namespace IdleFactory.Observable
{
  public class CombinedObservable<TSource, TTarget>(ICustomObservable<TSource>[] source, Func<IEnumerable<TSource>, TTarget> selector)
    : DerivedObservable<TTarget>(source.ToArray<ICustomObservable>())
  {
    protected override TTarget CalculateValue(bool attached)
    {
      return selector(source.Select(x => x.Value));
    }

    protected override void Attach()
    {
      base.Attach();

    }
  }
}
