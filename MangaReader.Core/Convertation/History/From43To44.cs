using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    protected override async Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var mangas = await context
          .Get<Manga.Mangas>()
          .Where(m => !m.Volumes.Any() && !m.Chapters.Any() && !m.Pages.Any())
          .ToListAsync()
          .ConfigureAwait(false);
        if (mangas.Any())
          process.ProgressState = ProgressState.Normal;

        foreach (var manga in mangas)
        {
          process.Percent += 100.0 / mangas.Count;

          try
          {
            await manga.Refresh().ConfigureAwait(false);
            await manga.UpdateContent().ConfigureAwait(false);
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
            await context.Save(manga).ConfigureAwait(false);
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