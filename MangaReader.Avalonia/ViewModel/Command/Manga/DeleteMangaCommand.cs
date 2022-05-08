using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Avalonia.Properties;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class DeleteMangaCommand : MultipleMangasBaseCommand
  {
    public override async Task Execute(IEnumerable<IManga> mangas)
    {
      var list = mangas.ToList();
      var isSingle = list.Count == 1;
      var text = isSingle ? string.Format("Удалить мангу {0}?", list[0].Name) :
        ("Удалить мангу?" + Environment.NewLine + string.Join(Environment.NewLine, list.Select(l => $" - {l}")));

      var result = await Services.Dialogs.ShowYesNoDialog("Удаление манги", text,
        "Манга и история её обновлений будет удалена.", isSingle ? $"Удалить папку {list[0].Folder}" : "Удалить связанные папки");

      if (result.dialogResult)
      {
        await Library.ThreadAction(DeleteManga(list, result.checkboxValue)).LogException().ConfigureAwait(true);
      }
    }

    protected async Task DeleteManga(List<IManga> list, bool deleteFolder)
    {
      foreach (var manga in list)
      {
        await Library.Remove(manga).ConfigureAwait(true);

        if (deleteFolder)
          DirectoryHelpers.DeleteDirectory(manga.GetAbsoluteFolderPath());
      }
    }

    public DeleteMangaCommand(SelectionModel mangaModels, LibraryViewModel library) : base(mangaModels, library)
    {
      this.Name = Strings.Manga_Action_Remove;
      this.Icon = "pack://application:,,,/Icons/Manga/delete.png";
    }
  }
}
