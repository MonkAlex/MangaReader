using System;
using System.Threading.Tasks;
using System.Windows;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class ExitCommand : BaseCommand
  {
    public static bool CommandRunned { get; set; }

    public override Task Execute(object parameter)
    {
      if (!CommandRunned)
      {
        CommandRunned = true;

        Log.Add("Application will be closed.");
        WindowModel.Instance.SaveWindowState();
        Client.Close();
        Application.Current.Exit += (sender, args) => Environment.Exit(args.ApplicationExitCode);
        Application.Current.Shutdown(0);
      }
      return Task.CompletedTask;
    }

    public ExitCommand()
    {
      this.Name = Strings.Library_Exit;
      this.Icon = "pack://application:,,,/Icons/Main/close_app.png";
    }
  }
}