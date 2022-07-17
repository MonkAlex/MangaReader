using System;
using System.Linq;
using System.Threading.Tasks;
using Hentaichan.Mangachan;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class BonusesChapter : TestClass
  {
    [Test]
    public async Task MangachanBonus()
    {
      var manga = await Mangas.CreateFromWeb(new Uri("https://manga-chan.me/manga/5335-the-breaker-new-waves.html")).ConfigureAwait(false);
      await manga.Parser.UpdateContent(manga).ConfigureAwait(false);
      var chapters = manga.Volumes.SelectMany(v => v.Container).ToList();
      Assert.AreEqual(1, chapters.Count(c => c.Number == 170));
      Assert.AreEqual(1, chapters.Count(c => c.Number == 170.1));
    }

    [Test, ReadManga]
    public async Task ReadmangaBonus()
    {
      var manga = await Mangas.CreateFromWeb(new Uri($"{Grouple.Constants.ReadmangaHost}animal_country")).ConfigureAwait(false);
      await manga.Parser.UpdateContent(manga).ConfigureAwait(false);
      var chapters = manga.Volumes.SelectMany(v => v.Container).OfType<Grouple.GroupleChapter>();
      Assert.AreEqual(1, chapters.Count(c => c.VolumeNumber == 14 && c.Number == 54));
      Assert.AreEqual(1, chapters.Count(c => c.VolumeNumber == 6 && c.Number == 22));
    }

    [Test, MintManga]
    public async Task MintmangaBonus()
    {
      var manga = await Mangas.CreateFromWeb(new Uri("https://mintmanga.live/harukana_receive")).ConfigureAwait(false);
      await manga.Parser.UpdateContent(manga).ConfigureAwait(false);
      var chapters = manga.Volumes.SelectMany(v => v.Container).OfType<Grouple.GroupleChapter>();
      Assert.AreEqual(1, chapters.Count(c => c.VolumeNumber == 1 && c.Number == 0));
    }
  }
}
