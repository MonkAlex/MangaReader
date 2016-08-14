using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class DeleteMangaCommand : MangaBaseCommand
  {
    public override void Execute(IManga parameter)
    {
      base.Execute(parameter);

      var text = string.Format("Удалить мангу {0}?", parameter.Name);
      var remove = Dialogs.ShowYesNoDialog("Удаление манги", text, "Манга и история её обновлений будет удалена.");
      if (remove)
        Library.Remove(parameter);
    }

    public DeleteMangaCommand()
    {
      this.Name = Strings.Manga_Action_Remove;
      this.Icon = "pack://application:,,,/Icons/Manga/delete.png";
    }
  }
}