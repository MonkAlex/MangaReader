using System;
using System.Linq;
using System.Threading.Tasks;
using Grouple;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture, ReadManga]
  public class ReadmangaStructure : TestClass
  {
    [Test]
    public async Task AddEmptyReadmanga()
    {
      var chapters = await GetCountOfChapters($"{Grouple.Constants.ReadmangaHost}_my_name_").ConfigureAwait(false);
      Assert.AreEqual(0, chapters);
    }

    [Test]
    public async Task AddReadmangaWithFractionNumber()
    {
      var manga = await Mangas.Create(new Uri($"{Grouple.Constants.ReadmangaHost}vanpanchmen")).ConfigureAwait(false);
      await new ReadmangaParser().UpdateContent(manga).ConfigureAwait(false);

      var chapters = manga.Volumes.SelectMany(v => v.Container);
      Assert.IsTrue(chapters.Any(c => c.Number == 209.2));
    }

    [Test]
    public async Task AddSingleReadmanga()
    {
      var chapters = await GetCountOfChapters($"{Grouple.Constants.ReadmangaHost}traeh").ConfigureAwait(false);
      Assert.AreEqual(1, chapters);
    }

    [Test]
    public async Task AddReadmangaWithoutExtra()
    {
      var chapters = await GetCountOfChapters($"{Grouple.Constants.ReadmangaHost}kuroshitsuji_dj___black_sheep").ConfigureAwait(false);
      Assert.AreEqual(4, chapters);
    }

    [Test]
    public async Task AddReadmangaWithExtra()
    {
      var chapters = await GetCountOfChapters($"{Grouple.Constants.ReadmangaHost}anima").ConfigureAwait(false);
      Assert.AreEqual(59, chapters);
    }

    [Test]
    public async Task AddReadmangaWithGoldChapters()
    {
      var manga = await Mangas.Create(new Uri($"{Grouple.Constants.ReadmangaHost}from_common_job_class_to_the_strongest_in_the_world")).ConfigureAwait(false);
      await new ReadmangaParser().UpdateContent(manga).ConfigureAwait(false);

      var volume = manga.Volumes.FirstOrDefault(v => v.Number == 4);
      Assert.NotNull(volume);
      Assert.AreEqual(7, volume.Container.Count);
    }

    private async Task<int> GetCountOfChapters(string url)
    {
      var manga = await Mangas.Create(new Uri(url)).ConfigureAwait(false);
      await new ReadmangaParser().UpdateContent(manga).ConfigureAwait(false);
      return manga.Volumes.Sum(v => v.Container.Count());
    }
  }
}
