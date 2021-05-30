using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MangaReader.Avalonia.View
{
  public partial class Changelog : UserControl
  {
    public Changelog()
    {
      InitializeComponent();
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
