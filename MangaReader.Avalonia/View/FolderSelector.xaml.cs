using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace MangaReader.Avalonia.View
{
  public class FolderSelector : UserControl
  {
    private TextBlock Path = null;

    public FolderSelector()
    {
      this.InitializeComponent();
      Path = this.Find<TextBlock>("Path");
      this.Find<Button>("Change").Click += OnChangeClick;
    }

    private async void OnChangeClick(object sender, RoutedEventArgs routedEventArgs)
    {
      var dialog = new OpenFolderDialog
      {
        DefaultDirectory = Path.Text,
        InitialDirectory = Path.Text,
        Title = "Select folder"
      };
      var result = await dialog.ShowAsync(Window.OpenWindows.FirstOrDefault(w => w.IsActive));
      if (!string.IsNullOrWhiteSpace(result))
        Path.Text = result;
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
