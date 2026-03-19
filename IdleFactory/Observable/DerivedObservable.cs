namespace IdleFactory.Observable
{
  public abstract class DerivedObservable<T>(params ICustomObservable[] derivedFrom) : BaseObservable<T>
  {
    protected override void AttachInternal()
    {
      foreach (var derived in derivedFrom)
      {
        derived.ValueChanged += this.OnDerivedValueChanged;
      }
    }

    protected override void DetachInternal()
    {
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
  }
}
