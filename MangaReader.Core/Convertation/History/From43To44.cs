using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation.History
{
  public class From43To44 : HistoryConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) &&
             Version.CompareTo(NHibernate.Repository.Get<DatabaseConfig>().Single().Version) > 0 &&
             process.Version.CompareTo(Version) >= 0;
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);
      var mangas = Repository.Get<Manga.Mangas>().ToList();
      if (mangas.Any())
        process.ProgressState = ProgressState.Normal;

      foreach (var manga in mangas.Where(m => !m.Volumes.Any() && !m.Chapters.Any() && !m.Pages.Any()))
      {
        process.Percent += 100.0 / mangas.Count;

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