using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using MangaReader.Avalonia.Services;
using MangaReader.Core.Services.Config;
using WindowState = Avalonia.Controls.WindowState;

namespace MangaReader.Avalonia.ViewModel.Command
{
  public class ShowMainWindowCommand : BaseCommand
  {
    private readonly ExplorerViewModel explorerViewModel;

    public override Task Execute(object parameter)
    {
      if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
      {
        var mainWindow = lifetime.MainWindow;
        if (mainWindow == null && ConfigStorage.Instance.AppConfig.StartMinimizedToTray)
        {
          SetLifetime(explorerViewModel, lifetime);
          mainWindow = lifetime.MainWindow;
        }

        if (mainWindow != null)
        {
          mainWindow.Show();
          mainWindow.ActivateWorkaround();
          if (mainWindow.WindowState == WindowState.Minimized)
            mainWindow.WindowState = WindowState.Normal;
        }
        else
        {
          Core.Services.Log.Error($"Try to show main window, but window not created. StartMinimizedToTray = {ConfigStorage.Instance.AppConfig.StartMinimizedToTray}");
        }
      }

      return Task.CompletedTask;
    }

    public static void SetLifetime(ExplorerViewModel explorer, IClassicDesktopStyleApplicationLifetime lifetime)
    {
      var window = new MainWindow();
      explorer.LoadingProcess.Status = window.Title;
      window.DataContext = explorer;
      lifetime.MainWindow = window;
    }

    public ShowMainWindowCommand(ExplorerViewModel explorerViewModel)
    {
      this.explorerViewModel = explorerViewModel;
    }
  }
}
