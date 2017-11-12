using System;
using System.Linq;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class MintmangaStructure : TestClass
  {
    [Test]
    public void AddMintmangaWithExtra()
    {
      var chapters = GetCountOfChapters("http://mintmanga.com/love_mate");
      Assert.AreEqual(168, chapters);
    }

    private int GetCountOfChapters(string url)
    {
      var manga = Mangas.Create(new Uri(url));
      new Grouple.Parser().UpdateContent(manga);
      return manga.Volumes.Sum(v => v.Container.Count());
    }
  }
}