using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using NHibernate.Linq;

namespace MangaReader.Core.Services
{
  public static class History
  {
    /// <summary>
    /// Добавление записи в историю.
    /// </summary>
    /// <param name="manga">Манга, к которой относится сообщение.</param>
    /// <param name="message">Сообщение.</param>
    public static void AddHistory(this Mangas manga, Uri message)
    {
      AddHistory(manga, new[] { message });
    }

    /// <summary>
    /// Добавление записей в историю.
    /// </summary>
    /// <param name="manga">Манга, к которой относятся сообщения.</param>
    /// <param name="messages">Сообщения.</param>
    public static void AddHistory(this Mangas manga, IEnumerable<Uri> messages)
    {
      var list = messages.Where(message => !manga.Histories.Any(h => h.Uri == message)).ToList();
      foreach (var message in list)
      {
        manga.Histories.Add(new MangaHistory(message));
      }
    }

    /// <summary>
    /// Вернуть загружаемые элементы, которые не записаны в историю.
    /// </summary>
    /// <param name="items">Список элементов.</param>
    /// <returns>Элементы, не записанные в историю.</returns>
    public static List<T> GetItemsWithoutHistory<T>(List<T> items) where T : IDownloadable
    {
      if (!items.Any())
        return items;

      var uris = items.Select(c => c.Uri).ToList();
      List<Uri> exists;
      using (var session = Mapping.OpenSession())
      {
        // В многопоточном коде нельзя обращаться к одной сессии.
        exists = session.Query<MangaHistory>().Where(h => uris.Contains(h.Uri)).Select(h => h.Uri).ToList();
      }
      return items.Where(c => uris.Except(exists).Contains(c.Uri)).ToList();
    }
  }
}
