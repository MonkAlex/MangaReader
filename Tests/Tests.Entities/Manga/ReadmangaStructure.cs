using System;
using System.Linq;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class ReadmangaStructure : TestClass
  {
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
      var chapters = GetCountOfChapters("http://readmanga.me/kuroshitsuji_dj___black_sheep");
      Assert.AreEqual(4, chapters);
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
      new Grouple.Parser().UpdateContent(manga);
      return manga.Volumes.Sum(v => v.Container.Count());
    }
  }
}