using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MangaReader.Avalonia.Services;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using WindowState = Avalonia.Controls.WindowState;

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

    protected override void HandleWindowStateChanged(WindowState state)
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        if (state == WindowState.Minimized && ConfigStorage.Instance.AppConfig.MinimizeToTray)
        {
          Log.Add($"App minimized to tray.");
          this.Hide();
        }

        if (state != WindowState.Minimized && ConfigStorage.Instance.AppConfig.MinimizeToTray)
        {
          Log.Add($"App restored from tray.");
          this.Show();
          // BUG: https://github.com/AvaloniaUI/Avalonia/issues/2994
          this.InvalidateMeasure();
        }
      }

      base.HandleWindowStateChanged(state);
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
