using System;
using Avalonia;
using MangaReader.Avalonia.Properties;
using MangaReader.Core;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command
{
  public class ExitCommand : BaseCommand
  {
    public static bool CommandRunned { get; set; }

    public override void Execute(object parameter)
    {
      if (!CommandRunned)
      {
        base.Execute(parameter);
        CommandRunned = true;

        Log.Add("Application will be closed.");
        Client.Close();
        Application.Current.OnExit += (sender, args) => Environment.Exit(0);
        Application.Current.Exit();
      }
    }

    public ExitCommand()
    {
      this.Name = Strings.Library_Exit;
      this.Icon = "pack://application:,,,/Icons/Main/close_app.png";
    }
  }
}