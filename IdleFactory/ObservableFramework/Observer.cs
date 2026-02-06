namespace IdleFactory.ObservableFramework
{
  public class Observer(params ICustomObservable[] observables)
  {
    private bool attached;

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
      foreach (var derived in observables)
      {
        derived.ValueChanged += this.OnDerivedValueChanged;
      }
    }

    protected virtual void Detach()
    {
      this.attached = false;
      foreach (var derived in observables)
      {
        derived.ValueChanged -= this.OnDerivedValueChanged;
      }
    }

    private void OnDerivedValueChanged(object? sender, EventArgs e)
    {
      this.valueChanged?.Invoke(sender, e);
    }
  }
}
