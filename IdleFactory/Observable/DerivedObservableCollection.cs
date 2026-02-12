using System.Collections;
using System.Diagnostics;

namespace IdleFactory.Observable
{
  public abstract class DerivedObservableCollection<TSource, TTarget>(IObservableCollection<TSource> sources) : IObservableCollection<TTarget>
  {
    private List<CachedTarget>? cachedTarget;

    private event EventHandler<ICollectionChangedEventArgs<TTarget>>? collectionChanged;

    public int Count => this.cachedTarget?.Sum(x => x.Targets.Count) ?? sources.SelectMany(this.GetTargets).Count();

    public event EventHandler<ICollectionChangedEventArgs<TTarget>>? CollectionChanged
    {
      add
      {
        if (this.collectionChanged == null)
        {
          this.Attach();
        }

        collectionChanged += value;
      }
      remove
      {
        collectionChanged -= value;
        if (this.collectionChanged == null)
        {
          this.Detach();
        }
      }
    }

    private void Attach()
    {
      sources.CollectionChanged += this.SourceCollectionChanged;
      this.cachedTarget = [.. sources.Select(x => new CachedTarget(x, this))];
    }

    private void Detach()
    {
      if (this.cachedTarget == null)
      {
        return;
      }

      foreach (var target in this.cachedTarget)
      {
        target.Dispose();
      }

      this.cachedTarget = null;
      sources.CollectionChanged -= this.SourceCollectionChanged;
    }

    private void SourceCollectionChanged(object? sender, ICollectionChangedEventArgs<TSource> e)
    {
      if (this.cachedTarget == null)
      {
        Debug.Fail("CollectionChanged triggered for not attached collection");
        return;
      }

      switch (e.Type)
      {
        case CollectionChangedType.Add:
          var newTarget = new CachedTarget(e.ChangedItem ?? throw new InvalidOperationException("ChangedItem is not set for add"), this);
          this.cachedTarget.Add(newTarget);
          foreach (var target in newTarget.Targets)
          {
            this.OnCollectionChanged(new CollectionChangedEventArgs<TTarget>(CollectionChangedType.Add, target));
          }

          break;
        case CollectionChangedType.Remove:
          var oldTarget = this.cachedTarget.FirstOrDefault(x => object.Equals(x.source, e.ChangedItem));
          if (oldTarget == null)
          {
            Debug.Fail("Target was removed that is no longer in the cached list");
            return;
          }

          this.cachedTarget.Remove(oldTarget);
          foreach (var target in oldTarget.Targets)
          {
            this.OnCollectionChanged(new CollectionChangedEventArgs<TTarget>(CollectionChangedType.Remove, target));
          }

          oldTarget.Dispose();
          break;
        case CollectionChangedType.Clear:
          foreach (var clearedTarget in this.cachedTarget)
          {
            clearedTarget.Dispose();
          }

          this.cachedTarget.Clear();
          this.OnCollectionChanged(new CollectionChangedEventArgs<TTarget>(CollectionChangedType.Clear, default));
          break;
      }
    }

    private void OnCollectionChanged(ICollectionChangedEventArgs<TTarget> e)
    {
      this.collectionChanged?.Invoke(this, e);
    }

    protected abstract IEnumerable<TTarget> GetTargets(TSource source);

    public IEnumerator<TTarget> GetEnumerator()
    {
      if (this.cachedTarget == null)
      {
        return sources.SelectMany(this.GetTargets).GetEnumerator();
      }

      return this.cachedTarget.SelectMany(x => x.Targets).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    private sealed class CachedTarget : IDisposable
    {
      public readonly TSource source;
      private readonly DerivedObservableCollection<TSource, TTarget> parent;
      private readonly List<TTarget> targets;

      public CachedTarget(TSource source, DerivedObservableCollection<TSource, TTarget> parent)
      {
        this.source = source;
        this.parent = parent;

        this.targets = [.. parent.GetTargets(source)];
        if (source is ICustomObservable customObservable)
        {
          customObservable.ValueChanged += this.SourceChanged;
        }
      }

      public IReadOnlyCollection<TTarget> Targets => this.targets;

      public void Dispose()
      {
        if (source is ICustomObservable customObservable)
        {
          customObservable.ValueChanged -= this.SourceChanged;
        }
      }

      private void SourceChanged(object? sender, EventArgs e)
      {
        var newTargets = this.parent.GetTargets(this.source);
        var targetsToRemove = this.targets.ToList();
        foreach (var target in newTargets)
        {
          if (targetsToRemove.Contains(target))
          {
            targetsToRemove.Remove(target);
          }

          this.targets.Add(target);
          this.parent.OnCollectionChanged(new CollectionChangedEventArgs<TTarget>(CollectionChangedType.Add, target));
        }

        foreach (var oldtarget in targetsToRemove)
        {
          this.targets.Remove(oldtarget);
          this.parent.OnCollectionChanged(new CollectionChangedEventArgs<TTarget>(CollectionChangedType.Remove, oldtarget));
        }
      }
    }
  }
}
