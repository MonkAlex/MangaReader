using System;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Manga.Acomic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Manga
{
  [TestClass]
  public class AcomicsStructure
  {
    [TestMethod]
    public void AddAcomicsOnlyPages()
    {
      var manga = GetManga("http://acomics.ru/~supersciencefriends");
      Assert.AreEqual(44, manga.Pages.Count);
      Assert.IsTrue(manga.OnlyPages);
    }

    [TestMethod]
    public void AddAcomicsChapters()
    {
      var manga = GetManga("http://acomics.ru/~hotblood");

      // Страниц, не привязанных к главам.
      Assert.AreEqual(7, manga.Pages.Count);

      // Глав
      Assert.AreEqual(3, manga.Chapters.Count);

      // Страниц в главах.
      Assert.AreEqual(267, manga.Chapters.Select(c => c.Pages.Count).Sum());

      Assert.IsTrue(manga.HasChapters);
      Assert.IsFalse(manga.HasVolumes);

      manga = GetManga("http://acomics.ru/~jonbot-vs-martha");
      Assert.AreEqual(4, manga.Chapters.Count);
    }

    [TestMethod]
    public void AddAcomicsVolume()
    {
      var manga = GetManga("http://acomics.ru/~strays");

      Assert.AreEqual(1, manga.Pages.Count);
      Assert.AreEqual(0, manga.Chapters.Count);
      Assert.AreEqual(3, manga.Volumes.Count);
      Assert.AreEqual(15, manga.Volumes.Select(c => c.Chapters.Count).Sum());
      Assert.AreEqual(389, manga.Volumes.SelectMany(c => c.Chapters).Select(c => c.Pages.Count).Sum());

      Assert.IsTrue(manga.HasChapters);
      Assert.IsTrue(manga.HasVolumes);

      manga = GetManga("http://acomics.ru/~girl-genius");
      Assert.AreEqual(14, manga.Volumes.Count);
      Assert.IsTrue(manga.HasVolumes);
    }

    private Acomics GetManga(string uri)
    {
      var manga = Mangas.CreateFromWeb(new Uri(uri)) as Acomics;
      Getter.UpdateContent(manga);
      return manga;
    }
  }
}