using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;

namespace MangaReader.Core.Convertation.History
{
  public class From43To44 : HistoryConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && this.CanConvertVersion(process);
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);
      using (var context = Repository.GetEntityContext())
      {
        var mangas = context.Get<Manga.Mangas>().Where(m => !m.Volumes.Any() && !m.Chapters.Any() && !m.Pages.Any()).ToList();
        if (mangas.Any())
          process.ProgressState = ProgressState.Normal;

        foreach (var manga in mangas)
        {
          process.Percent += 100.0 / mangas.Count;

          try
          {
            manga.Refresh();
            manga.UpdateContent();
            var history = manga.Histories.ToList();
            if (manga.Plugin.HistoryType != HistoryType.Page)
            {
              var volumeChapters = manga.Volumes.SelectMany(v => v.Container);
              SetDownloadableDate(history, volumeChapters.Concat(manga.Chapters));
            }
            else
            {
              var volumePages = manga.Volumes.SelectMany(v => v.Container).SelectMany(c => c.Container);
              var chapterPages = manga.Chapters.SelectMany(c => c.Container);
              SetDownloadableDate(history, volumePages.Concat(chapterPages).Concat(manga.Pages));
            }
            manga.Save();
          }
          catch (System.Exception ex)
          {
            Log.Exception(ex, $"Не удалось корректно обновить историю '{manga.Name}'.");
          }
        }
      }
    }

    private static void SetDownloadableDate(List<MangaHistory> history, IEnumerable<IDownloadable> downloadables)
    {
      foreach (var downloadable in downloadables)
      {
        var historyRecord = history.FirstOrDefault(h => h.Uri == downloadable.Uri);
        if (historyRecord != null)
          downloadable.DownloadedAt = historyRecord.Date;
      }
    }

    public From43To44()
    {
      this.CanReportProcess = true;
      this.Version = new Version(1, 44, 1);
    }
  }
}