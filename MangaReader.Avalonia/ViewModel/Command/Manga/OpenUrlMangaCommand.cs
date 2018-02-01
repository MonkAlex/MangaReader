using System.Collections.Generic;
using System.Diagnostics;
using MangaReader.Core.Manga;
using MangaReader.Avalonia.Properties;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class OpenUrlMangaCommand : MultipleMangasBaseCommand
  {
    public override void Execute(IEnumerable<IManga> mangas)
    {
      foreach (var manga in mangas)
      {
        Process.Start(manga.Uri.OriginalString);
      }
    }

    public override bool CanExecute(object parameter)
    {
      return true;
    }

    public OpenUrlMangaCommand(Explorer.LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_View;
      this.Icon = "pack://application:,,,/Icons/Manga/www.png";
      this.NeedRefresh = false;
    }
  }
}