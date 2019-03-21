using System.Diagnostics;
using System.Threading.Tasks;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class OpenWikiCommand : BaseCommand
  {
    public override Task Execute(object parameter)
    {
      Helper.StartUseShell("https://github.com/MonkAlex/MangaReader/wiki");
      return Task.CompletedTask;
    }

    public OpenWikiCommand()
    {
      this.Name = "Справка";
      this.Icon = "pack://application:,,,/Icons/Main/wiki.png";
    }
  }
}