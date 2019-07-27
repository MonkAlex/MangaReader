using System;
using Avalonia.Styling;

namespace MangaReader.Avalonia.View
{
  // BUG: https://github.com/AvaloniaUI/Avalonia/issues/2719
  public class ContextMenu : global::Avalonia.Controls.ContextMenu, IStyleable
  {
    Type IStyleable.StyleKey => typeof(global::Avalonia.Controls.ContextMenu);

    public ContextMenu()
    {
      this.DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, EventArgs e)
    {
      if (IsOpen && DataContext == null)
        Close();
    }
  }
}
