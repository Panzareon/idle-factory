namespace IdleFactory.Observable
{
  public class Observer(IObservableCollection<ICustomObservable> observables)
  {
    private bool attached;

    private List<ICustomObservable> attachedObservables = new();

    private event EventHandler? valueChanged;

    public event EventHandler ValueChanged
    {
      add
      {
        if (!this.attached)
        {
          this.Attach();
        }

        this.valueChanged += value;
      }
      remove
      {
        this.valueChanged -= value;
        if (this.valueChanged == null)
        {
          this.Detach();
        }
      }
    }

    protected virtual void Attach()
    {
      this.attached = true;
      observables.CollectionChanged += this.CollectionChanged;
      this.AttachTo(observables);
    }

    private void AttachTo(IEnumerable<ICustomObservable> observablesToAttach)
    {
      foreach (var derived in observablesToAttach)
      {
        derived.ValueChanged += this.OnDerivedValueChanged;
        attachedObservables.Add(derived);
      }
    }

    protected virtual void Detach()
    {
      this.attached = false;
      this.DetachFrom(observables);

      observables.CollectionChanged -= this.CollectionChanged;
    }

    private void DetachFrom(IEnumerable<ICustomObservable> observablesToDetach)
    {
      foreach (var derived in observablesToDetach)
      {
        derived.ValueChanged -= this.OnDerivedValueChanged;
        attachedObservables.Remove(derived);
      }
    }

    private void OnDerivedValueChanged(object? sender, EventArgs e)
    {
      this.valueChanged?.Invoke(sender, e);
    }

    private void CollectionChanged(object? sender, ICollectionChangedEventArgs<ICustomObservable> e)
    {
      if (e.Type == CollectionChangedType.Add)
      {
        this.AttachTo([e.ChangedItem ?? throw new InvalidOperationException("Changed item not set for add")]);
      }
      else if (e.Type == CollectionChangedType.Remove)
      {
        this.DetachFrom([e.ChangedItem ?? throw new InvalidOperationException("Changed item not set for remove")]);
      }
      else if (e.Type == CollectionChangedType.Clear)
      {
        this.DetachFrom([.. this.attachedObservables]);
      }
    }
  }
}
