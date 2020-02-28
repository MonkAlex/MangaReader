using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using MangaReader.Avalonia.Properties;
using MangaReader.Core;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command
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
        ExplorerViewModel.Instance.Dispose();
        Client.Close();
        //if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime && lifetime != parameter)
        //  lifetime.Shutdown();
        Environment.Exit(0);
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
