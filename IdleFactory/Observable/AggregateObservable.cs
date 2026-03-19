namespace IdleFactory.Observable
{
  public class AggregateObservable<TSource, TTarget>(IObservableCollection<TSource> source, Func<IObservableCollection<TSource>, TTarget> aggregate) : BaseObservable<TTarget>
  {
    protected override void AttachInternal()
    {
      source.CollectionChanged += this.OnCollectionChanged;
    }

    protected override void DetachInternal()
    {
      source.CollectionChanged -= this.OnCollectionChanged;
    }

    private void OnCollectionChanged(object? sender, ICollectionChangedEventArgs<TSource> e)
    {
      var newValue = this.CalculateValue(true);
      this.UpdateValue(newValue);
    }

    protected override TTarget CalculateValue(bool attached)
    {
      return aggregate(source);
    }
  }
}
