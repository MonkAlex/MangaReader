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
    public override void Execute(IManga parameter)
    {
      base.Execute(parameter);

      var text = string.Format("Удалить мангу {0}?", parameter.Name);

      var dialogResult = Dialogs.ShowYesNoDialog("Удаление манги", text,
        "Манга и история её обновлений будет удалена.", $"Удалить папку {parameter.Folder}");

      if (dialogResult.Item1)
      {
        Library.ThreadAction(() =>
        {
          Library.Remove(parameter);

          if (dialogResult.Item2)
            DirectoryHelpers.DeleteDirectory(parameter.GetAbsoulteFolderPath());
        });
      }
    }

    public DeleteMangaCommand(LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_Remove;
      this.Icon = "pack://application:,,,/Icons/Manga/delete.png";
    }
  }
}