using System;
using System.Collections.Generic;
using System.Linq;
using Dialogs.Controls;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Avalonia.Properties;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class DeleteMangaCommand : MultipleMangasBaseCommand
  {
    public override void Execute(IEnumerable<IManga> mangas)
    {
      var list = mangas.ToList();
      var isSingle = list.Count == 1;
      var text = isSingle ? string.Format("Удалить мангу {0}?", list[0].Name) :
        ("Удалить мангу?" + Environment.NewLine + string.Join(Environment.NewLine, list.Select(l => $" - {l}")));

      var dialog = new Dialogs.Avalonia.Dialog
      {
        Title = "Удаление манги",
        Description = text + Environment.NewLine + "Манга и история её обновлений будет удалена."
      };
      var deleteFolder = new BoolControl();
      dialog.Controls.Add(deleteFolder);
      deleteFolder.Name = isSingle ? $"Удалить папку {list[0].Folder}" : "Удалить связанные папки";
      var yes = dialog.Buttons.AddButton("Да");
      var no = dialog.Buttons.AddButton("Нет");

      if (dialog.Show() == yes)
      {
        Library.ThreadAction(() =>
        {
          foreach (var manga in list)
          {
            Library.Remove(manga);

            if (deleteFolder.Value)
              DirectoryHelpers.DeleteDirectory(manga.GetAbsoluteFolderPath());
          }
        }).LogException();
      }
    }

    public DeleteMangaCommand(Explorer.LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_Remove;
      this.Icon = "pack://application:,,,/Icons/Manga/delete.png";
    }
  }
}