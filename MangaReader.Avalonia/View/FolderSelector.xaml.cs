using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Data;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using ReactiveUI;

namespace MangaReader.Avalonia.View
{
  public class FolderSelector : UserControl
  {
    private TextBlock Path = null;
    private IDisposable lastBind = null;

    public static readonly DirectProperty<FolderSelector, string> FolderBindingPathProperty =
      AvaloniaProperty.RegisterDirect<FolderSelector, string>(nameof(FolderBindingPath), s => s.FolderBindingPath,
        (selector, s) => selector.FolderBindingPath = s);

    public string FolderBindingPath
    {
      get => folderBindingPath;
      set
      {
        lastBind?.Dispose();
        this.SetAndRaise(FolderBindingPathProperty, ref folderBindingPath, value);
        lastBind = Path.Bind(TextBlock.TextProperty, new Binding(FolderBindingPath, BindingMode.TwoWay));
      }
    }

    private string folderBindingPath;
    
    public FolderSelector()
    {
      this.InitializeComponent();
      Path = this.Find<TextBlock>("Path");
      this.Find<Button>("Change").Click += OnChangeClick;
    }

    private async void OnChangeClick(object sender, RoutedEventArgs routedEventArgs)
    {
      var absolutePath = DirectoryHelpers.GetAbsoulteFolderPath(this.Path.Text);
      var defaultPath = Directory.Exists(absolutePath) ? absolutePath : ConfigStorage.WorkFolder;

      var dialog = new OpenFolderDialog
      {
        DefaultDirectory = defaultPath,
        InitialDirectory = defaultPath,
        Title = "Select folder"
      };
      var result = await dialog.ShowAsync(Window.OpenWindows.FirstOrDefault(w => w.IsActive));
      if (!string.IsNullOrWhiteSpace(result))
      {
        var relativePath = DirectoryHelpers.GetRelativePath(ConfigStorage.WorkFolder, result);
        var value = relativePath.StartsWith(@"..\..\") ? result : relativePath;
        Path.Text = value;
      }
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
