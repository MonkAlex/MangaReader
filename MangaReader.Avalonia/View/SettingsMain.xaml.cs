using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MangaReader.Avalonia.View
{
  public class SettingsMain : UserControl
  {
    public SettingsMain()
    {
      this.InitializeComponent();
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
