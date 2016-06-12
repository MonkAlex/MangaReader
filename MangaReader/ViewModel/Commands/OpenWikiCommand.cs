using System.Diagnostics;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class OpenWikiCommand : BaseCommand
  {
    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      Process.Start("https://github.com/MonkAlex/MangaReader/wiki");
    }

    public OpenWikiCommand()
    {
      this.Name = "Справка";
      this.Icon = "pack://application:,,,/Icons/Main/wiki.png";
    }
  }
}