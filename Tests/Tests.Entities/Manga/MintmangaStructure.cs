using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class MintmangaStructure : TestClass
  {
    [Test]
    public async Task AddMintmangaWithExtra()
    {
      var chapters = await GetCountOfChapters("http://mintmanga.com/love_mate");
      Assert.AreEqual(168, chapters);
    }

    private async Task<int> GetCountOfChapters(string url)
    {
      var manga = Mangas.Create(new Uri(url));
      await new Grouple.Parser().UpdateContent(manga);
      return manga.Volumes.Sum(v => v.Container.Count());
    }
  }
}