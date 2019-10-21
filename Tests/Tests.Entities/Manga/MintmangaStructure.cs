using System;
using System.Linq;
using System.Threading.Tasks;
using Grouple;
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
      var chapters = await GetCountOfChapters("http://mintmanga.live/love_mate").ConfigureAwait(false);
      Assert.AreEqual(168, chapters);
    }

    private async Task<int> GetCountOfChapters(string url)
    {
      var manga = await Mangas.Create(new Uri(url)).ConfigureAwait(false);
      await new MintmangaParser().UpdateContent(manga).ConfigureAwait(false);
      return manga.Volumes.Sum(v => v.Container.Count());
    }
  }
}
