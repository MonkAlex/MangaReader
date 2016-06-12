using MangaReader.Core.Manga;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class ChangeUpdateMangaCommand : MangaBaseCommand
  {
    public override void Execute(Mangas manga)
    {
      base.Execute(manga);

      manga.NeedUpdate = !manga.NeedUpdate;
      manga.Save();

      this.Name = manga.NeedUpdate ? Strings.Manga_NotUpdate : Strings.Manga_Update;
      this.Icon = manga.NeedUpdate ? "pack://application:,,,/Icons/Manga/not_update.png" : "pack://application:,,,/Icons/Manga/need_update.png";
    }

    public ChangeUpdateMangaCommand(bool needUpdate)
    {
      this.Name = needUpdate ? Strings.Manga_NotUpdate : Strings.Manga_Update;
      this.Icon = needUpdate ? "pack://application:,,,/Icons/Manga/not_update.png" : "pack://application:,,,/Icons/Manga/need_update.png";
    }
  }
}