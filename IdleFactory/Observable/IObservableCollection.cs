namespace IdleFactory.Observable
{
  public enum CollectionChangedType
  {
    Add,

    Remove,

    Clear,
  }

  public interface ICollectionChangedEventArgs<out T>
  {
    public CollectionChangedType Type { get; }

    public T? ChangedItem { get; }
  }

  public class CollectionChangedEventArgs<T> : EventArgs, ICollectionChangedEventArgs<T>
  {
    public CollectionChangedEventArgs(CollectionChangedType type, T? changedItem)
    {
      this.Type = type;
      this.ChangedItem = changedItem;
    }

    public CollectionChangedType Type { get; }

    public T? ChangedItem { get; }
  }

  public interface IObservableCollection<out T> : IReadOnlyCollection<T>
  {
    event EventHandler<ICollectionChangedEventArgs<T>> CollectionChanged;
  }
}
