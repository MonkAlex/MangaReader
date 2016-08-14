using System.Diagnostics;
using MangaReader.Core.Manga;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class OpenUrlMangaCommand : MangaBaseCommand
  {
    public override void Execute(IManga manga)
    {
      base.Execute(manga);
      Process.Start(manga.Uri.OriginalString);
    }

    public override bool CanExecute(object parameter)
    {
      return true;
    }

    public OpenUrlMangaCommand()
    {
      this.Name = Strings.Manga_Action_View;
      this.Icon = "pack://application:,,,/Icons/Manga/www.png";
      this.NeedRefresh = false;
    }
  }
}