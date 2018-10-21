using System.Collections.Generic;
using MangaReader.Core.Manga;
using MangaReader.Avalonia.Properties;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class OpenUrlMangaCommand : MultipleMangasBaseCommand
  {
    public override void Execute(IEnumerable<IManga> mangas)
    {
      foreach (var manga in mangas)
      {
        Helper.StartUseShell(manga.Uri.OriginalString);
      }
    }

    public OpenUrlMangaCommand(Explorer.LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_View;
      this.Icon = "pack://application:,,,/Icons/Manga/www.png";
      this.NeedRefresh = false;
    }
  }
}