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
      var chapters = await GetCountOfChapters("https://readmanga.me/_my_name_").ConfigureAwait(false);
      Assert.AreEqual(0, chapters);
    }

    [Test]
    public async Task AddSingleReadmanga()
    {
      var chapters = await GetCountOfChapters("https://readmanga.me/traeh").ConfigureAwait(false);
      Assert.AreEqual(1, chapters);
    }

    [Test]
    public async Task AddReadmangaWithoutExtra()
    {
      var chapters = await GetCountOfChapters("https://readmanga.me/kuroshitsuji_dj___black_sheep").ConfigureAwait(false);
      Assert.AreEqual(4, chapters);
    }

    [Test]
    public async Task AddReadmangaWithExtra()
    {
      var chapters = await GetCountOfChapters("https://readmanga.me/anima").ConfigureAwait(false);
      Assert.AreEqual(59, chapters);
    }

    [Test]
    public async Task AddReadmangaWithGoldChapters()
    {
      var chapters = await GetCountOfChapters("https://readmanga.me/from_common_job_class_to_the_strongest_in_the_world").ConfigureAwait(false);
      Assert.AreEqual(29, chapters);
    }

    private async Task<int> GetCountOfChapters(string url)
    {
      var manga = await Mangas.Create(new Uri(url)).ConfigureAwait(false);
      await new ReadmangaParser().UpdateContent(manga).ConfigureAwait(false);
      return manga.Volumes.Sum(v => v.Container.Count());
    }
  }
}
