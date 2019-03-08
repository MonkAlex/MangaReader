using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class BonusesChapter : TestClass
  {
    [Test]
    public async Task MangachanBonus()
    {
      var manga = await Mangas.CreateFromWeb(new Uri("http://mangachan.me/manga/5335-the-breaker-new-waves.html")).ConfigureAwait(false);
      await manga.Parser.UpdateContent(manga).ConfigureAwait(false);
      var chapters = manga.Volumes.SelectMany(v => v.Container).ToList();
      Assert.AreEqual(1, chapters.Count(c => c.Number == 170));
      Assert.AreEqual(1, chapters.Count(c => c.Number == 170.1));
    }

    [Test]
    public async Task ReadmangaBonus()
    {
      var manga = await Mangas.CreateFromWeb(new Uri("http://readmanga.me/animal_country")).ConfigureAwait(false);
      await manga.Parser.UpdateContent(manga).ConfigureAwait(false);
      var chapters = manga.Volumes.SelectMany(v => v.Container).OfType<Grouple.GroupleChapter>();
      Assert.AreEqual(1, chapters.Count(c => c.VolumeNumber == 14 && c.Number == 54));
      Assert.AreEqual(1, chapters.Count(c => c.VolumeNumber == 6 && c.Number == 22));
    }

    [Test]
    public async Task MintmangaBonus()
    {
      using (var context = Repository.GetEntityContext())
      {
        var uri = new Uri("http://mintmanga.com/haruka_na_receive");
        var toRemove = context.Get<IManga>().Where(m => m.Uri == uri).ToList();
        foreach (var remove in toRemove)
          context.Delete(remove);
        var manga = await Mangas.CreateFromWeb(uri).ConfigureAwait(false);
        await manga.Parser.UpdateContent(manga).ConfigureAwait(false);
        var chapters = manga.Volumes.SelectMany(v => v.Container).OfType<Grouple.GroupleChapter>();
        Assert.AreEqual(1, chapters.Count(c => c.VolumeNumber == 1 && c.Number == 0));
      }
    }
  }
}