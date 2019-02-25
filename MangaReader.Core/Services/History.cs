using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using NHibernate.Exceptions;
using NHibernate.Linq;

namespace MangaReader.Core.Services
{
  public static class History
  {

    public static void FilterActiveElements(Mangas manga)
    {
      var histories = manga.Histories.ToList();

      bool PageMustBeLoaded(MangaPage p)
      {
        return p.DownloadedAt == null && histories.All(m => m.Uri != p.Uri);
      }

      bool ChapterMustBeLoaded(Chapter ch)
      {
        if (ch.Container.Any())
          return histories.All(m => m.Uri != ch.Uri) || ch.Container.Any(PageMustBeLoaded);

        return ch.DownloadedAt == null && histories.All(m => m.Uri != ch.Uri);
      }

      bool VolumeMustBeLoaded(Volume v)
      {
        return v.Container.Any(ChapterMustBeLoaded);
      }

      manga.ActivePages = manga.ActivePages.Where(PageMustBeLoaded).ToList();
      manga.ActiveChapters = manga.ActiveChapters.Where(ChapterMustBeLoaded).ToList();
      manga.ActiveVolumes = manga.ActiveVolumes.Where(VolumeMustBeLoaded).ToList();
    }

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

      var uris = internalContainer.Select(c => c.Uri).Distinct().ToList();

      List<Uri> exists;
      using (var session = Mapping.GetStatelessSession())
      {
        exists = session.Query<MangaHistory>().Where(h => uris.Contains(h.Uri)).Select(h => h.Uri).ToList();
      }
      
      foreach (var item in internalContainer.OfType<IDownloadableContainer<IDownloadable>>())
      {
        var wh = GetItemsWithoutHistory(item);
        if (wh.Any())
          exists.Remove(item.Uri);
      }
      
      var historyNotFound = internalContainer.Where(c => uris.Except(exists).Contains(c.Uri)).ToList();
      return historyNotFound.Where(c => c.DownloadedAt == null).ToList();
    }
  }
}
