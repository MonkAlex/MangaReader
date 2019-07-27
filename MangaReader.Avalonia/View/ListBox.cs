using System;
using Avalonia.Input;
using Avalonia.Styling;

namespace MangaReader.Avalonia.View
{
  // BUG: https://github.com/AvaloniaUI/Avalonia/issues/2730
  public class ListBox : global::Avalonia.Controls.ListBox, IStyleable
  {
    Type IStyleable.StyleKey => typeof(global::Avalonia.Controls.ListBox);

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
      base.OnPointerPressed(e);
      e.Handled = false;
    }
  }
}
