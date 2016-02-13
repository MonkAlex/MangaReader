using System.Windows;
using MangaReader.Properties;
using MangaReader.Services;

namespace MangaReader.ViewModel.Commands
{
  public class ExitCommand : BaseCommand
  {
    public override string Name { get { return Strings.Library_Exit; } }

    public override void Execute(object parameter)
    {
      var window = parameter as Window;
      if (window != null)
      {
        Log.Add("Window will be closed.");
        window.Close();
      }
      else
      {
        Log.Add("Application will be closed.");
        Client.Close();
      }
      Application.Current.Shutdown(0);
    }
  }
}