using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using MangaReader.Avalonia.Services;
using MangaReader.Avalonia.ViewModel;
using MangaReader.Core;
using MangaReader.Core.Convertation;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Core.Update;

namespace MangaReader.Avalonia
{
  public class MainWindow : Window
  {
    public MainWindow()
    {
      this.InitializeComponent();
      Title = "Loading...";
      ConfigStorage.Instance.ViewConfig.UpdateWindowState(this);
      App.AttachDevTools(this);
    }

    protected override bool HandleClosing()
    {
      ConfigStorage.Instance.ViewConfig.SaveWindowState(this);
      return base.HandleClosing();
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
