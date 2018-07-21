using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation.History
{
  public class FromBaseTo32 : HistoryConverter
  {
    /// <summary>
    /// Ссылка на файл лога.
    /// </summary>
    private static readonly string HistoryFile = Path.Combine(ConfigStorage.WorkFolder, "history");

    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && File.Exists(HistoryFile);
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var histories = new List<MangaHistory>();

      var serializedStrings = Serializer<List<string>>.Load(HistoryFile);
      var isStringList = serializedStrings != null;

      var serializedMangaHistoris = Serializer<List<MangaHistory>>.Load(HistoryFile);
      var isMangaHistory = serializedMangaHistoris != null;

      var strings = File.Exists(HistoryFile) ? new List<string>(File.ReadAllLines(HistoryFile)) : new List<string>();

      // ReSharper disable CSharpWarnings::CS0612
#pragma warning disable CS0612 // Obsolete методы используются для конвертации
      if (!isMangaHistory && !isStringList)
        histories = MangaHistory.CreateHistories(strings);
      if (!isMangaHistory && isStringList)
        histories = MangaHistory.CreateHistories(serializedStrings);
      if (isMangaHistory && !isStringList)
        histories = serializedMangaHistoris;
#pragma warning restore CS0612
      // ReSharper restore CSharpWarnings::CS0612

      using (var context = Repository.GetEntityContext())
      {
        var mangas = context.Get<IManga>().ToList();
        var historyInDb = Repository.GetStateless<MangaHistory>().Select(h => h.Uri).ToList();
        histories = histories.Where(h => !historyInDb.Contains(h.Uri)).Distinct().ToList();
        if (histories.Any())
          process.ProgressState = ProgressState.Normal;

        TryFillHistory(process, context, mangas, histories);
        
        // Актуализируем старые ссылки, с учетом переездов сайтов.
        var mangaSettings = Repository.GetStateless<MangaSetting>();
        foreach (var history in histories)
        {
          var settings = mangaSettings.Where(s => s.MangaSettingUris.Select(u => u.Host).Contains(history.Uri.Host));
          foreach (var setting in settings)
          {
            var hosts = setting.MangaSettingUris.Where(s => !Equals(s, setting.MainUri)).Select(s => s.Host).ToList();
            foreach (var host in hosts)
            {
              if (history.MangaUrl != null && history.MangaUrl.Contains(host))
                history.MangaUrl = history.MangaUrl.Replace(host, setting.MainUri.Host);
              if (history.Uri.Host == host)
              {
                var builder = new UriBuilder(history.Uri) { Host = setting.MainUri.Host, Port = -1 };
                history.Uri = builder.Uri;
              }
            }
          }
        }

        TryFillHistory(process, context, mangas, histories);

        using (var tranc = context.OpenTransaction())
        {
          foreach (var history in histories.GroupBy(h => new Uri(h.Uri, "..")))
          {
            var manga = Manga.Mangas.Create(history.Key);
            if (manga == null)
              continue;

            manga.Refresh();
            if (manga.Uri == history.Key)
              continue;

            var uri = manga.Uri;
            manga = mangas.FirstOrDefault(m => m.Uri == uri);
            if (manga == null)
              continue;

            foreach (var record in history)
              record.Uri = new Uri(record.Uri.OriginalString.Replace(history.Key.OriginalString.TrimEnd('/'), manga.Uri.OriginalString.TrimEnd('/')));
            manga.Histories.AddRange(history);
            context.AddToTransaction(manga);
          }
          tranc.Commit();
        }
      }

      Backup.MoveToBackup(HistoryFile);
    }

    private static void TryFillHistory(IProcess process, RepositoryContext context, List<IManga> mangas, List<MangaHistory> histories)
    {
      using (var tranc = context.OpenTransaction())
      {
        foreach (var manga in mangas)
        {
          process.Percent += 100.0 / mangas.Count;
          var mangaHistory = histories.Where(h => h.MangaUrl == manga.Uri.OriginalString ||
                                                  h.Uri.OriginalString.Contains(manga.Uri.OriginalString + "/")).ToList();
          manga.Histories.AddRange(mangaHistory);
          histories.RemoveAll(h => mangaHistory.Contains(h));
          context.AddToTransaction(manga);
        }
        tranc.Commit();
      }
    }

    public FromBaseTo32()
    {
      this.CanReportProcess = true;
      this.Version = new Version(1, 0, 2);
    }
  }
}