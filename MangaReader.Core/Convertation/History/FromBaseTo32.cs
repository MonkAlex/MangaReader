using System.Collections.Generic;
using System.Diagnostics;
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

      var mangas = Repository.Get<Manga.Mangas>().ToList();
      var historyInDb = Repository.Get<MangaHistory>().Select(h => h.Uri).ToList();
      histories = histories.Where(h => !historyInDb.Contains(h.Uri)).Distinct().ToList();
      if (histories.Any())
        process.ProgressState = ProgressState.Normal;

      using (var tranc = Mapping.Session.BeginTransaction())
      {
        foreach (var manga in mangas)
        {
          process.Percent += 100.0 / mangas.Count;
          var mangaHistory = histories.Where(h => h.MangaUrl == manga.Uri.OriginalString ||
            h.Uri.OriginalString.Contains(manga.Uri.OriginalString)).ToList();
          foreach (var history in mangaHistory)
            manga.Histories.Add(history);
        }
        tranc.Commit();
      }

      Backup.MoveToBackup(HistoryFile);
    }
  }
}