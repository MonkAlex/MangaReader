﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Avalonia.Properties;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class HistoryClearMangaCommand : MultipleMangasBaseCommand
  {
    public override async Task Execute(IEnumerable<IManga> mangas)
    {
      var list = mangas.ToList();

      var text = list.Count == 1 ? $"Удалить историю {list[0].Name}?" :
        ("Удалить историю?" + Environment.NewLine + string.Join(Environment.NewLine, list.Select(l => $" - {l}")));
      var clear = await Services.Dialogs.ShowYesNoDialog("Удаление истории", text, "После удаления истории манга будет скачиваться целиком.");
      if (clear)
      {
        foreach (var manga in list)
        {
          manga.ClearHistory();
        }

        using (var context = Repository.GetEntityContext())
          await list.SaveAll(context).ConfigureAwait(true);
      }
    }

    public HistoryClearMangaCommand(SelectionModel mangaModels, LibraryViewModel library) : base(mangaModels, library)
    {
      this.Name = Strings.Manga_Action_Remove + " историю";
      this.Icon = "pack://application:,,,/Icons/Manga/history_clear.png";
    }
  }
}
