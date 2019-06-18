using System;
using System.ComponentModel;
using Grouple;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;
using NUnit.Framework;
using SortDescription = MangaReader.Core.Services.Config.SortDescription;

namespace Tests.Entities.Services
{
  [TestFixture]
  public class LibraryFilterTests
  {
    [Test]
    public void TestMangaComparer(
      [Values(nameof(ILibraryFilterableItem.Name), nameof(ILibraryFilterableItem.Created), nameof(ILibraryFilterableItem.DownloadedAt))] string property,
      [Values] ListSortDirection direction,
      [Values] bool isManga)
    {
      var comparer = new LibraryFilter();
      comparer.SortDescription = new SortDescription(property, direction);

      var utcNow = DateTime.UtcNow;

      (int compareDifferent, int compareEquals) CompareManga()
      {
        IManga manga1 = new Readmanga() { Name = "A-Name", Created = utcNow, DownloadedAt = utcNow };
        IManga manga2 = new Readmanga()
          { Name = "b-name", Created = utcNow.AddMinutes(1), DownloadedAt = utcNow.AddMinutes(1) };
        IManga manga3 = new Readmanga() { Name = "a-name", Created = utcNow, DownloadedAt = utcNow };
        var i = comparer.Compare(manga1, manga2);
        var compareEquals1 = comparer.Compare(manga1, manga3);
        return (i, compareEquals1);
      }

      (int compareDifferent, int compareEquals) CompareProxy()
      {
        ILibraryFilterableItem manga1 = new MangaProxy(new Readmanga() { Name = "A-Name", Created = utcNow, DownloadedAt = utcNow });
        ILibraryFilterableItem manga2 = new MangaProxy(new Readmanga() { Name = "b-name", Created = utcNow.AddMinutes(1), DownloadedAt = utcNow.AddMinutes(1) });
        ILibraryFilterableItem manga3 = new MangaProxy(new Readmanga() { Name = "a-name", Created = utcNow, DownloadedAt = utcNow });
        var i = comparer.Compare(manga1, manga2);
        var compareEquals1 = comparer.Compare(manga1, manga3);
        return (i, compareEquals1);
      }

      var (compareDifferent, compareEquals) = isManga ? CompareManga() : CompareProxy();

      if (direction == ListSortDirection.Descending)
        Assert.Greater(compareDifferent, 0);
      else
        Assert.Less(compareDifferent, 0);

      Assert.AreEqual(0, compareEquals);
    }

    private class MangaProxy : ILibraryFilterableItem
    {
      public MangaProxy(IManga manga)
      {
        this.manga = manga;
      }

      private IManga manga;
      public DateTime? Created
      {
        get => manga.Created;
        set => manga.Created = value;
      }

      public DateTime? DownloadedAt
      {
        get => manga.DownloadedAt;
        set => manga.DownloadedAt = value;
      }

      public string Name
      {
        get => manga.Name;
        set => manga.Name = value;
      }
    }
  }
}
