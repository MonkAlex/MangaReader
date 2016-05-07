using System.Diagnostics;
using System.Windows.Data;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class OpenUrlMangaCommand : MangaBaseCommand
  {
    public override void Execute(Mangas manga)
    {
      base.Execute(manga);
      Process.Start(manga.Uri.OriginalString);
    }

    public override bool CanExecute(object parameter)
    {
      return true;
    }

    public OpenUrlMangaCommand(ListCollectionView view) : base(view)
    {
      this.Name = Strings.Manga_Action_View;
    }
  }
}