using MangaReader.Core.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;
using Ookii.Dialogs.Wpf;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class HistoryClearMangaCommand : MangaBaseCommand
  {
    public override void Execute(Mangas manga)
    {
      base.Execute(manga);

      var dialog = new TaskDialog();
      dialog.WindowTitle = "Удаление истории";
      dialog.MainInstruction = string.Format("Удалить историю {0}?", manga.Name);
      dialog.Content = "После удаления истории манга будет скачиваться целиком.";
      dialog.Buttons.Add(new TaskDialogButton(ButtonType.Yes));
      dialog.Buttons.Add(new TaskDialogButton(ButtonType.No));
      if (dialog.ShowDialog(WindowHelper.Owner).ButtonType == ButtonType.Yes)
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