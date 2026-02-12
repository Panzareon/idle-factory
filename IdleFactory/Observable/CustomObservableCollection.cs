using System.Collections;

namespace IdleFactory.Observable
{
  public class CustomObservableCollection<TItem> : ICollection<TItem>, IObservableCollection<TItem>
  {
    private readonly List<TItem> items;

    public CustomObservableCollection(IEnumerable<TItem> items)
    {
      this.items = [.. items];
    }

    public int Count => this.items.Count;

    public bool IsReadOnly => false;

    public event EventHandler<ICollectionChangedEventArgs<TItem>>? CollectionChanged;

    public void Add(TItem item)
    {
      this.items.Add(item);
      this.CollectionChanged?.Invoke(this, new CollectionChangedEventArgs<TItem>(CollectionChangedType.Add, item));
    }

    public void Clear()
    {
      this.items.Clear();
      this.CollectionChanged?.Invoke(this, new CollectionChangedEventArgs<TItem>(CollectionChangedType.Clear, default));
    }

    public bool Contains(TItem item)
    {
      return this.items.Contains(item);
    }

    public void CopyTo(TItem[] array, int arrayIndex)
    {
      this.items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<TItem> GetEnumerator()
    {
      return this.items.GetEnumerator();
    }

    public bool Remove(TItem item)
    {
      if (this.items.Remove(item))
      {
        this.CollectionChanged?.Invoke(this, new CollectionChangedEventArgs<TItem>(CollectionChangedType.Remove, item));
        return true;
      }

      return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
