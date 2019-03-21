using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class OpenUrlMangaCommand : MultipleMangasBaseCommand
  {
    public override Task Execute(IEnumerable<IManga> mangas)
    {
      foreach (var manga in mangas)
      {
        Helper.StartUseShell(manga.Uri.OriginalString);
      }

      return Task.CompletedTask;
    }

    public override bool CanExecute(object parameter)
    {
      return true;
    }

    public OpenUrlMangaCommand(MainPageModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_View;
      this.Icon = "pack://application:,,,/Icons/Manga/www.png";
      this.NeedRefresh = false;
    }
  }
}