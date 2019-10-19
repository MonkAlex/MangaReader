using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MangaReader.Avalonia.Services;

namespace MangaReader.Avalonia.ViewModel.Command
{
  public class ShowMainWindowCommand : BaseCommand
  {
    public override Task Execute(object parameter)
    {
      if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
      {
        var mainWindow = lifetime.MainWindow;
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
