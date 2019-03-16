using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class HistoryClearMangaCommand : MultipleMangasBaseCommand
  {
    public override void Execute(IEnumerable<IManga> mangas)
    {
      var list = mangas.ToList();

      var text = list.Count == 1 ? $"Удалить историю {list[0].Name}?" :
        ("Удалить историю?" + Environment.NewLine + string.Join(Environment.NewLine, list.Select(l => $" - {l}")));
      var clear = Dialogs.ShowYesNoDialog("Удаление истории", text, "После удаления истории манга будет скачиваться целиком.");
      if (clear)
      {
        foreach (var manga in list)
        {
          manga.ClearHistory();
        }

        using (var context = Repository.GetEntityContext())
          list.SaveAll(context).GetAwaiter().GetResult();
      }
    }

    public HistoryClearMangaCommand(MainPageModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_Remove + " историю";
      this.Icon = "pack://application:,,,/Icons/Manga/history_clear.png";
    }
  }
}