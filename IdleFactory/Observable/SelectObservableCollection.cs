
namespace IdleFactory.Observable
{
  public class SelectObservableCollection<TSource, TTarget>(IObservableCollection<TSource> source, Func<TSource, TTarget> selector)
    : DerivedObservableCollection<TSource, TTarget>(source)
  {
    protected override IEnumerable<TTarget> GetTargets(TSource source)
    {
      return [selector(source)];
    }
  }
}
