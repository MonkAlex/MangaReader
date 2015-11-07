using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NHibernate.Linq;
using MangaReader.Manga;
using MangaReader.Services.Config;
using Environment = MangaReader.Mapping.Environment;

namespace MangaReader.Services
{
  static class History
  {
    /// <summary>
    /// Ссылка на файл лога.
    /// </summary>
    private static readonly string HistoryFile = ConfigStorage.WorkFolder + @".\history";

    /// <summary>
    /// Добавление записи в историю.
    /// </summary>
    /// <param name="manga">Манга, к которой относится сообщение.</param>
    /// <param name="message">Сообщение.</param>
    /// <remarks>Используется для сохранения важной информации - открывает новое соединение, дорогая операция.</remarks>
    public static void AddHistory(this Mangas manga, Uri message)
    {
      using (var session = Environment.OpenSession())
      using (var tranc = session.BeginTransaction())
      {
        if (manga.Histories.Any(h => h.Uri == message))
          return;

        manga.Histories.Add(new MangaHistory(message));
        tranc.Commit();
      }
    }

    /// <summary>
    /// Вернуть загружаемые элементы, которые не записаны в историю.
    /// </summary>
    /// <param name="items">Список элементов.</param>
    /// <returns>Элементы, не записанные в историю.</returns>
    public static List<T> GetItemsWithoutHistory<T>(List<T> items) where T: IDownloadable
    {
      List<T> result;
      var uris = items.Select(c => c.Uri).ToList();

      using (var session = Environment.OpenSession())
      {
        result = items.Where(c => uris.Except(session.Query<MangaHistory>().Where(h => uris.Contains(h.Uri)).Select(h => h.Uri)).Contains(c.Uri)).ToList();
      }

      return result;
    }

    /// <summary>
    /// Сконвертировать в новый формат.
    /// </summary>
    public static void Convert(ConverterProcess process)
    {
      if (!File.Exists(HistoryFile))
        return;

      // ReSharper disable CSharpWarnings::CS0612
      var histories = new List<MangaHistory>();

      var serializedStrings = Serializer<List<string>>.Load(HistoryFile);
      var isStringList = serializedStrings != null;

      var serializedMangaHistoris = Serializer<List<MangaHistory>>.Load(HistoryFile);
      var isMangaHistory = serializedMangaHistoris != null;

      var strings = File.Exists(HistoryFile) ? new List<string>(File.ReadAllLines(HistoryFile)) : new List<string>();

      if (!isMangaHistory && !isStringList)
#pragma warning disable CS0612 // Obsolete методы используются для конвертации
        histories = MangaHistory.CreateHistories(strings);
      if (!isMangaHistory && isStringList)
        histories = MangaHistory.CreateHistories(serializedStrings);
#pragma warning restore CS0612
      if (isMangaHistory && !isStringList)
        histories = serializedMangaHistoris;

      using (var session = Environment.OpenSession())
      {
        var mangas = session.Query<Mangas>().ToList();
        var historyInDb = session.Query<MangaHistory>().Select(h => h.Uri).ToList();
        histories = histories.Where(h => !historyInDb.Contains(h.Uri)).Distinct().ToList();
        if (process != null && histories.Any())
          process.IsIndeterminate = false;

        foreach (var manga in mangas)
        {
          if (process != null)
            process.Percent += 100.0 / mangas.Count;
          using (var tranc = session.BeginTransaction())
          {
            var mangaHistory = histories.Where(h => h.MangaUrl == manga.Uri.OriginalString || 
              h.Uri.OriginalString.Contains(manga.Uri.OriginalString)).ToList();
            foreach (var history in mangaHistory)
              manga.Histories.Add(history);
            tranc.Commit();
          }
        }
      }
      // ReSharper restore CSharpWarnings::CS0612

      Backup.MoveToBackup(HistoryFile);
    }
  }
}
