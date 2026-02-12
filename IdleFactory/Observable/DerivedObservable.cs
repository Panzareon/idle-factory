namespace IdleFactory.Observable
{
  public abstract class DerivedObservable<T>(params ICustomObservable[] derivedFrom) : ICustomObservable<T>
  {
    private bool attached;

    private T? cachedValue;

    private event EventHandler? valueChanged;

    public T Value
    {
      get => this.attached ? this.cachedValue! : this.CalculateValue(false);
    }

    protected abstract T CalculateValue(bool attached);

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
      this.cachedValue = this.CalculateValue(true);
      this.attached = true;
      foreach (var derived in derivedFrom)
      {
        derived.ValueChanged += this.OnDerivedValueChanged;
      }
    }

    protected virtual void Detach()
    {
      this.attached = false;
      foreach (var derived in derivedFrom)
      {
        derived.ValueChanged -= this.OnDerivedValueChanged;
      }
    }

    private void OnDerivedValueChanged(object? sender, EventArgs e)
    {
      var newValue = this.CalculateValue(true);
      this.UpdateValue(newValue);
    }

    protected void UpdateValue(T newValue)
    {
      if (!object.Equals(this.cachedValue, newValue))
      {
        this.cachedValue = newValue;
        this.valueChanged?.Invoke(this, EventArgs.Empty);
      }
    }
  }
}
