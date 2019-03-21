using System.Threading.Tasks;
using System.Windows;
using MangaReader.ViewModel.Commands.Primitives;
using WindowState = System.Windows.WindowState;

namespace MangaReader.ViewModel.Commands
{
  public class ShowMainWindowCommand : BaseCommand
  {
    public override Task Execute(object parameter)
    {
      var mainWindow = Application.Current.MainWindow;
      if (mainWindow != null)
      {
        mainWindow.Show();
        mainWindow.Activate();
        if (mainWindow.WindowState == WindowState.Minimized)
          mainWindow.WindowState = WindowState.Normal;
      }

      return Task.CompletedTask;
    }
  }
}