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
    private readonly ExplorerViewModel explorerViewModel;
    public static bool CommandRunned { get; set; }

    public override Task Execute(object parameter)
    {
      if (!CommandRunned)
      {
        CommandRunned = true;

        Log.Add("Application will be closed.");
        explorerViewModel.Dispose();
        Client.Close();
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime && lifetime != parameter)
          lifetime.Shutdown();
      }

      return Task.CompletedTask;
    }

    public ExitCommand(ExplorerViewModel explorerViewModel)
    {
      this.explorerViewModel = explorerViewModel;
      this.Name = Strings.Library_Exit;
      this.Icon = "pack://application:,,,/Icons/Main/close_app.png";
    }
  }
}
