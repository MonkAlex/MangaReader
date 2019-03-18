using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class DeleteMangaCommand : MultipleMangasBaseCommand
  {
    public override void Execute(IEnumerable<IManga> mangas)
    {
      var list = mangas.ToList();
      var isSingle = list.Count == 1;
      var text = isSingle ? string.Format("Удалить мангу {0}?", list[0].Name) :
        ("Удалить мангу?" + Environment.NewLine + string.Join(Environment.NewLine, list.Select(l => $" - {l}")));

      var dialogResult = Dialogs.ShowYesNoDialog("Удаление манги", text,
        "Манга и история её обновлений будет удалена.", isSingle ? $"Удалить папку {list[0].Folder}" : "Удалить связанные папки");

      if (dialogResult.Item1)
      {
        Library.ThreadAction(async () =>
        {
          foreach (var manga in list)
          {
            await Library.Remove(manga).ConfigureAwait(true);

            if (dialogResult.Item2)
              DirectoryHelpers.DeleteDirectory(manga.GetAbsoluteFolderPath());
          }
        }).LogException();
      }
    }

    public DeleteMangaCommand(MainPageModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_Remove;
      this.Icon = "pack://application:,,,/Icons/Manga/delete.png";
    }
  }
}