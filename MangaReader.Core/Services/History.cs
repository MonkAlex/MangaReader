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
    /// <param name="container">Контейнер.</param>
    /// <returns>Элементы, не записанные в историю.</returns>
    public static IEnumerable<T> GetItemsWithoutHistory<T>(IDownloadableContainer<T> container) where T : IDownloadable
    {
      if (!container.Container.Any())
        return container.Container;

      var uris = container.Container.Select(c => c.Uri).ToList();
      List<Uri> exists;
      using (var session = Mapping.OpenSession())
      {
        // В многопоточном коде нельзя обращаться к одной сессии.
        exists = session.Query<MangaHistory>().Where(h => uris.Contains(h.Uri)).Select(h => h.Uri).ToList();
      }
      foreach (var item in container.Container.OfType<IDownloadableContainer<IDownloadable>>())
      {
        var wh = GetItemsWithoutHistory(item);
        if (wh.Any())
          exists.Remove(item.Uri);
      }
      return container.Container.Where(c => uris.Except(exists).Contains(c.Uri)).ToList();
    }
  }
}
