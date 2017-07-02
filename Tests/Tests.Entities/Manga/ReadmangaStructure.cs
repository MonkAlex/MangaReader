using System;
using System.Linq;
using Grouple;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class ReadmangaStructure : TestClass
  {
    private Parser parser = new Parser();

    [Test]
    public void AddEmptyReadmanga()
    {
      var chapters = GetCountOfChapters("http://readmanga.me/_my_name_");
      Assert.AreEqual(0, chapters);
    }

    [Test]
    public void AddSingleReadmanga()
    {
      var chapters = GetCountOfChapters("http://readmanga.me/traeh");
      Assert.AreEqual(1, chapters);
    }

    [Test]
    public void AddReadmangaWithoutExtra()
    {
      var chapters = GetCountOfChapters("http://readmanga.me/hack__xxxx");
      Assert.AreEqual(10, chapters);
    }

    [Test]
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