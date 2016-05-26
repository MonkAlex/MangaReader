using System;
using MangaReader.Core.Manga.Grouple;
using MangaReader.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Manga
{
  [TestClass]
  public class ReadmangaChapterCounts
  {
    [TestMethod]
    public void AddEmptyReadmanga()
    {
      var chapters = GetCountOfChapters("http://readmanga.me/_my_name_");
      Assert.AreEqual(chapters, 0);
    }

    [TestMethod]
    public void AddSingleReadmanga()
    {
      var chapters = GetCountOfChapters("http://readmanga.me/traeh");
      Assert.AreEqual(chapters, 1);
    }

    [TestMethod]
    public void AddReadmangaWithoutExtra()
    {
      var chapters = GetCountOfChapters("http://readmanga.me/hack__xxxx");
      Assert.AreEqual(chapters, 10);
    }

    [TestMethod]
    public void AddReadmangaWithExtra()
    {
      var chapters = GetCountOfChapters("http://readmanga.me/anima");
      Assert.AreEqual(chapters, 59);
    }

    private int GetCountOfChapters(string url)
    {
      return Getter.GetLinksOfMangaChapters(Page.GetPage(new Uri(url))).Count;
    }
  }
}