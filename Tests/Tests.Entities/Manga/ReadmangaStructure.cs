using System;
using System.Linq;
using System.Threading.Tasks;
using Grouple;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class ReadmangaStructure : TestClass
  {
    [Test]
    public async Task AddEmptyReadmanga()
    {
      var chapters = await GetCountOfChapters("http://readmanga.me/_my_name_").ConfigureAwait(false);
      Assert.AreEqual(0, chapters);
    }

    [Test]
    public async Task AddSingleReadmanga()
    {
      var chapters = await GetCountOfChapters("http://readmanga.me/traeh").ConfigureAwait(false);
      Assert.AreEqual(1, chapters);
    }

    [Test]
    public async Task AddReadmangaWithoutExtra()
    {
      var chapters = await GetCountOfChapters("http://readmanga.me/kuroshitsuji_dj___black_sheep").ConfigureAwait(false);
      Assert.AreEqual(4, chapters);
    }

    [Test]
    public async Task AddReadmangaWithExtra()
    {
      var chapters = await GetCountOfChapters("http://readmanga.me/anima").ConfigureAwait(false);
      Assert.AreEqual(59, chapters);
    }

    private async Task<int> GetCountOfChapters(string url)
    {
      var manga = Mangas.Create(new Uri(url));
      await new Parser().UpdateContent(manga).ConfigureAwait(false);
      return manga.Volumes.Sum(v => v.Container.Count());
    }
  }
}