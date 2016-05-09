using MangaReader.Core.Manga;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class HistoryClearMangaCommand : MangaBaseCommand
  {
    public override void Execute(Mangas manga)
    {
      base.Execute(manga);
      manga.Histories.Clear();
      manga.Save();
    }

    public HistoryClearMangaCommand()
    {
      this.Name = Strings.Manga_Action_Remove + " историю";
    }
  }
}