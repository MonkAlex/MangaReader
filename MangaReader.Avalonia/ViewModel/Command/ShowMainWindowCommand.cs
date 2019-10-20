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
    public override Task Execute(object parameter)
    {
      if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
      {
        var mainWindow = lifetime.MainWindow;
        if (mainWindow == null && ConfigStorage.Instance.AppConfig.StartMinimizedToTray)
        {
          var explorer = ExplorerViewModel.Instance;
          var window = new MainWindow();
          explorer.LoadingProcess.Status = window.Title;
          window.DataContext = explorer;
          lifetime.MainWindow = window;
          mainWindow = lifetime.MainWindow;
        }

        if (mainWindow != null)
        {
          mainWindow.Show();
          mainWindow.ActivateWorkaround();
          if (mainWindow.WindowState == WindowState.Minimized)
            mainWindow.WindowState = WindowState.Normal;
        }
      }

      return Task.CompletedTask;
    }
  }
}
