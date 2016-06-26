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
