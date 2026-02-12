using System.Collections;

namespace IdleFactory.Observable
{
  public class ConcatObservableCollection<TTarget>(IObservableCollection<TTarget> collection1, IObservableCollection<TTarget> collection2) : IObservableCollection<TTarget>
  {
    public int Count => collection1.Count + collection2.Count;

    public event EventHandler<ICollectionChangedEventArgs<TTarget>> CollectionChanged
    {
      add
      {
        collection1.CollectionChanged += value;
        collection2.CollectionChanged += value;
      }

      remove
      {
        collection1.CollectionChanged -= value;
        collection2.CollectionChanged -= value;
      }
    }

    public IEnumerator<TTarget> GetEnumerator()
    {
      return collection1.AsEnumerable().Concat(collection2).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
