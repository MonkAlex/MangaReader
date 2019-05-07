using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.CRUD
{
  [TestFixture]
  public class Manga : TestClass
  {
    [Test]
    public async Task MangaCreateDelete()
    {
      var newManga = await Builder.CreateReadmanga().ConfigureAwait(false);
      var mangaId = newManga.Id;
      Assert.AreNotEqual(0, newManga.Id);

      using (var context = Repository.GetEntityContext())
      {
        var fromDb = await context.Get<IManga>().FirstOrDefaultAsync(m => m.Id == mangaId).ConfigureAwait(false);
        Assert.AreNotEqual(null, fromDb);

        await Builder.DeleteReadmanga(newManga).ConfigureAwait(false);
        Assert.AreEqual(0, newManga.Id);

        fromDb = await context.Get<IManga>().FirstOrDefaultAsync(m => m.Id == mangaId).ConfigureAwait(false);
        Assert.AreEqual(null, fromDb);
      }
    }

    [Test]
    public async Task MangaReadUpdate()
    {
      var newManga = await Builder.CreateAcomics().ConfigureAwait(false);
      var mangaId = newManga.Id;
      var name = newManga.Name;
      var updatedName = name + "2";

      using (var context = Repository.GetEntityContext())
      {
        var manga = await context.Get<IManga>().SingleAsync(m => m.Id == mangaId).ConfigureAwait(false);
        Assert.AreEqual(name, manga.Name);
        manga.Name = updatedName;
        await context.Save(manga).ConfigureAwait(false);
      }

      using (var context = Repository.GetEntityContext())
      {
        var manga = await context.Get<IManga>().SingleAsync(m => m.Id == mangaId).ConfigureAwait(false);
        Assert.AreEqual(updatedName, manga.Name);
      }
    }

    [Test]
    public async Task MangaChangeUri()
    {
      var newManga = await Builder.CreateAcomics().ConfigureAwait(false);
      var mangaId = newManga.Id;
      var uri = newManga.Uri;
      var testUri = new Uri(uri, "test");

      using (var context = Repository.GetEntityContext())
      {
        var manga = await context.Get<IManga>().SingleAsync(m => m.Id == mangaId).ConfigureAwait(false);
        manga.Uri = testUri;
        await context.Save(manga).ConfigureAwait(false);
        Assert.AreNotEqual(uri, manga.Uri);
      }

      using (var context = Repository.GetEntityContext())
      {
        var manga = await context.Get<IManga>().SingleAsync(m => m.Id == mangaId).ConfigureAwait(false);
        manga.Uri = Builder.ReadmangaUri;
        Assert.CatchAsync<MangaSaveValidationException>(async () => await context.Save(manga).ConfigureAwait(false));
        Assert.AreEqual(Builder.ReadmangaUri, manga.Uri);
      }

      using (var context = Repository.GetEntityContext())
      {
        var manga = await context.Get<IManga>().SingleAsync(m => m.Id == mangaId).ConfigureAwait(false);
        Assert.AreEqual(testUri, manga.Uri);
      }
    }
  }
}
