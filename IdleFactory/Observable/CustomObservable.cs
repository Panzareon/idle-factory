using IdleFactory.Observable;

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

    public static Observer OnAnyChanged<TCollection>(this TCollection observables)
      where TCollection : IObservableCollection<ICustomObservable>
    {
      return new Observer(observables);
    }

    public static CustomObservableCollection<TItem> AsObservableCollection<TItem>(this IEnumerable<TItem> observables)
      where TItem : ICustomObservable
    {
      return new CustomObservableCollection<TItem>(observables);
    }

    public static IObservableCollection<TTarget> Select<TSource, TTarget>(this IObservableCollection<TSource> sources, Func<TSource, TTarget> selector)
    {
      return new SelectObservableCollection<TSource, TTarget>(sources, selector);
    }

    public static IObservableCollection<TTarget> Concat<TTarget>(this IObservableCollection<TTarget> collection1, IObservableCollection<TTarget> collection2)
    {
      return new ConcatObservableCollection<TTarget>(collection1, collection2);
    }
  }
}
