namespace IdleFactory.Observable
{
  public abstract class BaseObservable<T> : ICustomObservable<T>
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

    protected void UpdateValue(T newValue)
    {
      if (!object.Equals(this.cachedValue, newValue))
      {
        this.cachedValue = newValue;
        this.valueChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    protected void Attach()
    {
      this.cachedValue = this.CalculateValue(true);
      this.attached = true;
      this.AttachInternal();
    }
    protected void Detach()
    {
      this.attached = false;
      this.DetachInternal();
    }

    protected abstract void AttachInternal();
    protected abstract void DetachInternal();
  }
}
