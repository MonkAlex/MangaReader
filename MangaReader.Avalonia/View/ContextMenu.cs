using System;
using Avalonia.Controls.Generators;
using Avalonia.Styling;
using DynamicData.Binding;

namespace MangaReader.Avalonia.View
{
  // BUG: https://github.com/AvaloniaUI/Avalonia/issues/2719
  public class ContextMenu : global::Avalonia.Controls.ContextMenu, IStyleable
  {
    Type IStyleable.StyleKey => typeof(global::Avalonia.Controls.ContextMenu);

    // BUG: https://github.com/AvaloniaUI/Avalonia/issues/3468
    protected override void OnContainersDematerialized(ItemContainerEventArgs e)
    {
      try
      {
        base.OnContainersDematerialized(e);
      }
      catch { }
    }

    public ContextMenu()
    {
      this.WhenPropertyChanged(menu => menu.DataContext).Subscribe(OnDataContextChanged);
    }

    private void OnDataContextChanged(PropertyValue<ContextMenu, object> obj)
    {
      if (IsOpen)
        Close();
    }
  }
}
