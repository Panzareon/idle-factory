namespace IdleFactory.Observable
{
  internal class ObservableSelectMany<TSource, TTarget>(ICustomObservable<TSource> source, Func<TSource, ICustomObservable<TTarget>> selector)
    : DerivedObservable<TTarget>(source)
  {
    private ICustomObservable<TTarget>? currentCachedTarget;

    protected override TTarget CalculateValue(bool attached)
    {
      var target = selector(source.Value);
      if (attached)
      {
        this.currentCachedTarget?.ValueChanged -= this.TargetValueChanged;
        this.currentCachedTarget = target;
        this.currentCachedTarget.ValueChanged += this.TargetValueChanged;
      }

      return target.Value;
    }

    protected override void Detach()
    {
      base.Detach();
      this.currentCachedTarget?.ValueChanged -= this.TargetValueChanged;
      this.currentCachedTarget = null;
    }

    private void TargetValueChanged(object? sender, EventArgs e)
    {
      if (this.currentCachedTarget == null)
      {
        throw new InvalidOperationException("Cache target value changed, but is null");
      }

      this.UpdateValue(this.currentCachedTarget.Value);
    }
  }
}