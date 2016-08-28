using System;
using System.Linq;
using Grouple;
using MangaReader.Core.Manga;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Manga
{
  [TestClass]
  public class MintmangaStructure
  {
    private Parser parser = new Parser();

    [TestMethod]
    public void AddMintmangaWithExtra()
    {
      var chapters = GetCountOfChapters("http://mintmanga.com/love_mate");
      Assert.AreEqual(168, chapters);
    }

    private int GetCountOfChapters(string url)
    {
      var manga = Mangas.Create(new Uri(url));
      parser.UpdateContent(manga);
      return manga.Volumes.Sum(v => v.Chapters.Count);
    }
  }
}