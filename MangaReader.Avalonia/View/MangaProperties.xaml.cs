using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MangaReader.Avalonia.View
{
  public class MangaProperties : UserControl
  {
    public MangaProperties()
    {
      this.InitializeComponent();
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
