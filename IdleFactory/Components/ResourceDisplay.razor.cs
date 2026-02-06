using IdleFactory.Data.Main;
using Microsoft.AspNetCore.Components;

namespace IdleFactory.Components
{
  public partial class ResourceDisplay : IDisposable
  {
    [Parameter]
    public required Resource Resource
    {
      get;
      set
      {
        if (field != null)
        {
          field.PropertyChanged -= this.PropertyChanged;
        }

        field = value;
        if (field != null)
        {
          field.PropertyChanged += this.PropertyChanged;
        }
      }
    }

    public void Dispose()
    {
      if (this.Resource != null)
      {
        this.Resource.PropertyChanged -= this.PropertyChanged;
      }
    }

    private void PropertyChanged(object? sender, EventArgs e)
    {
      this.StateHasChanged();
    }
  }
}
