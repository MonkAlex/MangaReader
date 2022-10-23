using System;
using System.Linq;
using System.Threading.Tasks;
using Acomics;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using NHibernate.Linq;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class AcomicsStructure : TestClass
  {
    [Test]
    public async Task AddAcomicsOnlyPages()
    {
      var manga = await GetManga("https://acomics.ru/~supersciencefriends").ConfigureAwait(false);
      Assert.AreEqual(44, manga.Pages.Count);
      Assert.IsTrue(manga.OnlyPages);
    }

    [Test]
    public async Task AddAcomicsChapters()
    {
      var manga = await GetManga("https://acomics.ru/~hotblood").ConfigureAwait(false);

      // Страниц, не привязанных к главам.
      Assert.AreEqual(7, manga.Pages.Count);

      // Глав
      Assert.AreEqual(3, manga.Chapters.Count);

      // Страниц в главах.
      Assert.AreEqual(268, manga.Chapters.Select(c => c.Container.Count).Sum());

      Assert.IsTrue(manga.HasChapters);
      Assert.IsFalse(manga.HasVolumes);

      manga = await GetManga("https://acomics.ru/~jonbot-vs-martha").ConfigureAwait(false);
      Assert.AreEqual(4, manga.Chapters.Count);
    }

    [Test]
    public async Task AddAcomicsVolume()
    {
      var manga = await GetManga("https://acomics.ru/~strays").ConfigureAwait(false);

      Assert.AreEqual(1, manga.Pages.Count);
      Assert.AreEqual(0, manga.Chapters.Count);
      Assert.AreEqual(3, manga.Volumes.Count);
      Assert.AreEqual(15, manga.Volumes.Select(c => c.Container.Count()).Sum());
      Assert.AreEqual(399, manga.Volumes.SelectMany(c => c.Container).Select(c => c.Container.Count).Sum());

      Assert.IsTrue(manga.HasChapters);
      Assert.IsTrue(manga.HasVolumes);

      manga = await GetManga("https://acomics.ru/~girl-genius").ConfigureAwait(false);
      Assert.AreEqual(14, manga.Volumes.Count);
      Assert.IsTrue(manga.HasVolumes);
    }

    private async Task<Acomics.Acomics> GetManga(string uri)
    {
      var classUri = new Uri(uri);
      using (var context = Repository.GetEntityContext())
        foreach (var forDelete in await context.Get<Acomics.Acomics>().Where(m => m.Uri == classUri).ToListAsync().ConfigureAwait(false))
          await context.Delete(forDelete).ConfigureAwait(false);

      var manga = await Mangas.CreateFromWeb(classUri).ConfigureAwait(false) as Acomics.Acomics;
      await new Parser().UpdateContent(manga).ConfigureAwait(false);
      return manga;
    }
  }
}
