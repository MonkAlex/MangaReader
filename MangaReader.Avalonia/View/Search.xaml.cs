using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MangaReader.Avalonia.View
{
  public class Search : UserControl
  {
    public Search()
    {
      this.InitializeComponent();
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
