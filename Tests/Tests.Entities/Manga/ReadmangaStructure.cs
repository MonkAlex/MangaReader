using System;
using System.Linq;
using Grouple;
using MangaReader.Core.Manga;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Manga
{
  [TestClass]
  public class ReadmangaStructure
  {
    private Parser parser = new Parser();

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
      var manga = Mangas.Create(new Uri(url));
      parser.UpdateContent(manga);
      return manga.Volumes.Sum(v => v.Chapters.Count);
    }
  }
}