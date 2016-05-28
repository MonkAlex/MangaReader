using System;
using MangaReader.Core.Manga.Grouple;
using MangaReader.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Manga
{
  [TestClass]
  public class ReadmangaStructure
  {
    [TestMethod]
    public void AddEmptyReadmanga()
    {
      var chapters = GetCountOfChapters("http://readmanga.me/_my_name_");
      Assert.AreEqual(0, chapters);
    }

    [TestMethod]
    public void AddSingleReadmanga()
    {
      var chapters = GetCountOfChapters("http://readmanga.me/traeh");
      Assert.AreEqual(1, chapters);
    }

    [TestMethod]
    public void AddReadmangaWithoutExtra()
    {
      var chapters = GetCountOfChapters("http://readmanga.me/hack__xxxx");
      Assert.AreEqual(10, chapters);
    }

    [TestMethod]
    public void AddReadmangaWithExtra()
    {
      var chapters = GetCountOfChapters("http://readmanga.me/anima");
      Assert.AreEqual(59, chapters);
    }

    private int GetCountOfChapters(string url)
    {
      return Getter.GetLinksOfMangaChapters(Page.GetPage(new Uri(url))).Count;
    }
  }
}