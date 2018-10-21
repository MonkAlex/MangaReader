using System;
using System.Linq;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class AcomicsStructure : TestClass
  {
    [Test]
    public void AddAcomicsOnlyPages()
    {
      var manga = GetManga("https://acomics.ru/~supersciencefriends");
      Assert.AreEqual(44, manga.Pages.Count);
      Assert.IsTrue(manga.OnlyPages);
    }

    [Test]
    public void AddAcomicsChapters()
    {
      var manga = GetManga("https://acomics.ru/~hotblood");

      // Страниц, не привязанных к главам.
      Assert.AreEqual(7, manga.Pages.Count);

      // Глав
      Assert.AreEqual(3, manga.Chapters.Count);

      // Страниц в главах.
      Assert.AreEqual(268, manga.Chapters.Select(c => c.Container.Count).Sum());

      Assert.IsTrue(manga.HasChapters);
      Assert.IsFalse(manga.HasVolumes);

      manga = GetManga("https://acomics.ru/~jonbot-vs-martha");
      Assert.AreEqual(4, manga.Chapters.Count);
    }

    [Test]
    public void AddAcomicsVolume()
    {
      var manga = GetManga("https://acomics.ru/~strays");

      Assert.AreEqual(1, manga.Pages.Count);
      Assert.AreEqual(0, manga.Chapters.Count);
      Assert.AreEqual(3, manga.Volumes.Count);
      Assert.AreEqual(15, manga.Volumes.Select(c => c.Container.Count()).Sum());
      Assert.AreEqual(399, manga.Volumes.SelectMany(c => c.Container).Select(c => c.Container.Count).Sum());

      Assert.IsTrue(manga.HasChapters);
      Assert.IsTrue(manga.HasVolumes);

      manga = GetManga("https://acomics.ru/~girl-genius");
      Assert.AreEqual(14, manga.Volumes.Count);
      Assert.IsTrue(manga.HasVolumes);
    }

    private Acomics.Acomics GetManga(string uri)
    {
      var manga = Mangas.CreateFromWeb(new Uri(uri)) as Acomics.Acomics;
      new Acomics.Parser().UpdateContent(manga);
      return manga;
    }
  }
}