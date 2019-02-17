using System;
using System.Linq;
using System.Threading.Tasks;
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
      var chapters = await GetCountOfChapters("http://readmanga.me/_my_name_");
      Assert.AreEqual(0, chapters);
    }

    [Test]
    public async Task AddSingleReadmanga()
    {
      var chapters = await GetCountOfChapters("http://readmanga.me/traeh");
      Assert.AreEqual(1, chapters);
    }

    [Test]
    public async Task AddReadmangaWithoutExtra()
    {
      var chapters = await GetCountOfChapters("http://readmanga.me/kuroshitsuji_dj___black_sheep");
      Assert.AreEqual(4, chapters);
    }

    [Test]
    public async Task AddReadmangaWithExtra()
    {
      var chapters = await GetCountOfChapters("http://readmanga.me/anima");
      Assert.AreEqual(59, chapters);
    }

    private async Task<int> GetCountOfChapters(string url)
    {
      var manga = Mangas.Create(new Uri(url));
      await new Grouple.Parser().UpdateContent(manga);
      return manga.Volumes.Sum(v => v.Container.Count());
    }
  }
}