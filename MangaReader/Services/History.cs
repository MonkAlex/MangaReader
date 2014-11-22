using System.Collections.Generic;
using System.IO;
using System.Linq;
using NHibernate.Linq;
using MangaReader.Manga;

namespace MangaReader.Services
{
  static class History
  {
    /// <summary>
    /// Ссылка на файл лога.
    /// </summary>
    private static readonly string HistoryPath = Settings.WorkFolder + @".\history";

    /// <summary>
    /// Добавление записи в историю.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    public static void Add(string message)
    {
      using (var session = Mapping.Environment.SessionFactory.OpenSession())
      using (var tranc = session.BeginTransaction())
      {
        if (session.Query<MangaHistory>().Where(h => h.Url == message).Any())
          return;

        var history = new MangaHistory(message);
        session.Save(history);
        tranc.Commit();
      }
    }

    /// <summary>
    /// Сконвертировать в новый формат.
    /// </summary>
    public static void Convert(ConverterProcess process)
    {
      if (!File.Exists(HistoryPath))
        return;

      var histories = new List<MangaHistory>();

      var serializedStrings = Serializer<List<string>>.Load(HistoryPath);
      var isStringList = serializedStrings != null;

      var serializedMangaHistoris = Serializer<List<MangaHistory>>.Load(HistoryPath);
      var isMangaHistory = serializedMangaHistoris != null;

      var strings = File.Exists(HistoryPath) ? new List<string>(File.ReadAllLines(HistoryPath)) : new List<string>();

      if (!isMangaHistory && !isStringList)
        histories = MangaHistory.CreateHistories(strings);
      if (!isMangaHistory && isStringList)
        histories = MangaHistory.CreateHistories(serializedStrings);
      if (isMangaHistory && !isStringList)
        histories = serializedMangaHistoris;

      var session = Mapping.Environment.Session;
      var mangas = session.Query<Mangas>().ToList();
      var historyInDb = session.Query<MangaHistory>().Select(h => h.Url).ToList();
      histories = histories.Where(h => !historyInDb.Contains(h.Url)).Distinct().ToList();
      if (process != null && histories.Any())
        process.IsIndeterminate = false;

      using (var tranc = session.BeginTransaction())
      {
        foreach (var history in histories)
        {
          if (process != null)
            process.Percent += 100.0 / histories.Count;
          if (history.Manga == null)
            history.Manga = mangas.SingleOrDefault(m => m.Url == history.MangaUrl);
          // TODO: надо решить что делать с невалидной историей.
          //if (history.Manga != null)
          session.Save(history);
        }
        tranc.Commit();
      }

      File.Move(HistoryPath, HistoryPath + ".dbak");
    }

    /// <summary>
    /// Получить историю.
    /// </summary>
    /// <param name="manga">Манга, история которой нужна.</param>
    /// <returns>Перечисление сообщений из истории.</returns>
    public static List<MangaHistory> Get(Mangas manga)
    {
      return Mapping.Environment.Session.Query<MangaHistory>().Where(h => h.Manga.Id == manga.Id).ToList();
    }
  }
}
