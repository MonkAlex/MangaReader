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
    public static List<T> GetItemsWithoutHistory<T>(IDownloadableContainer<T> container) where T : IDownloadable
    {
      var internalContainer = container.Container.ToList();
      if (!internalContainer.Any())
        return internalContainer;

      var uris = internalContainer.Select(c => c.Uri).ToList();
      
      // В многопоточном коде нельзя обращаться к одной сессии.
      var exists = Mapping.GetSession().Query<MangaHistory>().Where(h => uris.Contains(h.Uri)).Select(h => h.Uri).ToList();

      foreach (var item in internalContainer.OfType<IDownloadableContainer<IDownloadable>>())
      {
        var wh = GetItemsWithoutHistory(item);
        if (wh.Any())
          exists.Remove(item.Uri);
      }
      
      return internalContainer.Where(c => uris.Except(exists).Contains(c.Uri)).ToList();
    }
  }
}
