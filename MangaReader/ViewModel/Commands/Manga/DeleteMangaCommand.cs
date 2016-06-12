using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;
using Ookii.Dialogs.Wpf;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class DeleteMangaCommand : MangaBaseCommand
  {
    public override void Execute(Mangas parameter)
    {
      base.Execute(parameter);

      var dialog = new TaskDialog();
      dialog.WindowTitle = "Удаление манги";
      dialog.MainInstruction = string.Format("Удалить мангу {0}?", parameter.Name);
      dialog.Content = "Манга и история её обновлений будет удалена.";
      dialog.Buttons.Add(new TaskDialogButton(ButtonType.Yes));
      dialog.Buttons.Add(new TaskDialogButton(ButtonType.No));
      if (dialog.ShowDialog(WindowHelper.Owner).ButtonType == ButtonType.Yes)
      {
        Library.Remove(parameter);
      }
    }

    public DeleteMangaCommand()
    {
      this.Name = Strings.Manga_Action_Remove;
      this.Icon = "pack://application:,,,/Icons/Manga/delete.png";
    }
  }
}