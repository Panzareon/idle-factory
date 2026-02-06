using IdleFactory.ObservableFramework;

namespace IdleFactory.Observable
{
  public static class CustomObservable
  {
    public static ICustomObservable<TTarget> Select<TSource, TTarget>(this ICustomObservable<TSource> source, Func<TSource, TTarget> selector)
    {
      return new ObservableSelector<TSource, TTarget>(source, selector);
    }

    public static ICustomObservable<TTarget> CombineLatest<TSource, TTarget>(this IEnumerable<ICustomObservable<TSource>> source, Func<IEnumerable<TSource>, TTarget> selector)
    {
      return new CombinedObservable<TSource, TTarget>([.. source], selector);
    }

    public static ICustomObservable<TTarget> SelectMany<TSource, TTarget>(this ICustomObservable<TSource> source, Func<TSource, ICustomObservable<TTarget>> selector)
    {
      return new ObservableSelectMany<TSource, TTarget>(source, selector);
    }

    public static ICustomObservable<T> Return<T>(T value)
    {
      return new FixedValue<T>
      {
        Value = value,
      };
    }

    public static Observer OnAnyChanged(this IEnumerable<ICustomObservable> observables)
    {
      return new Observer([.. observables]);
    }
  }
}
