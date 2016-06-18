using MangaReader.Core.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class HistoryClearMangaCommand : MangaBaseCommand
  {
    public override void Execute(Mangas manga)
    {
      base.Execute(manga);

      var text = string.Format("Удалить историю {0}?", manga.Name);
      var clear = Dialogs.ShowYesNoDialog("Удаление истории", text, "После удаления истории манга будет скачиваться целиком.");
      if (clear)
      {
        manga.Histories.Clear();
        manga.Save();
      }
    }

    public HistoryClearMangaCommand()
    {
      this.Name = Strings.Manga_Action_Remove + " историю";
      this.Icon = "pack://application:,,,/Icons/Manga/history_clear.png";
    }
  }
}