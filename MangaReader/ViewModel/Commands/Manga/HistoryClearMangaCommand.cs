using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class HistoryClearMangaCommand : MangaBaseCommand
  {
    public override void Execute(IManga manga)
    {
      base.Execute(manga);

      var text = string.Format("Удалить историю {0}?", manga.Name);
      var clear = Dialogs.ShowYesNoDialog("Удаление истории", text, "После удаления истории манга будет скачиваться целиком.");
      if (clear)
      {
        manga.ClearHistory();
        manga.Save();
      }
    }

    public HistoryClearMangaCommand(LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_Remove + " историю";
      this.Icon = "pack://application:,,,/Icons/Manga/history_clear.png";
    }
  }
}