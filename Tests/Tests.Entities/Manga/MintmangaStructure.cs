using System;
using MangaReader.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Manga
{
  [TestClass]
  public class MintmangaStructure
  {

    [TestMethod]
    public void AddMintmangaWithExtra()
    {
      var chapters = GetCountOfChapters("http://mintmanga.com/love_mate");
      Assert.AreEqual(168, chapters);
    }

    private int GetCountOfChapters(string url)
    {
      return Grouple.Getter.GetLinksOfMangaChapters(Page.GetPage(new Uri(url))).Count;
    }
  }
}