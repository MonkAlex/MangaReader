using System;
using System.Linq;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using NUnit.Framework;

namespace Tests.Entities.CRUD
{
  [TestFixture]
  public class Manga : TestClass
  {
    [Test]
    public void MangaCreateDelete()
    {
      var newManga = Builder.CreateReadmanga();
      var mangaId = newManga.Id;
      Assert.AreNotEqual(0, newManga.Id);

      using (var context = Repository.GetEntityContext())
      {
        var fromDb = context.Get<IManga>().FirstOrDefault(m => m.Id == mangaId);
        Assert.AreNotEqual(null, fromDb);

        Builder.DeleteReadmanga(newManga);
        Assert.AreEqual(0, newManga.Id);

        fromDb = context.Get<IManga>().FirstOrDefault(m => m.Id == mangaId);
        Assert.AreEqual(null, fromDb);
      }
    }

    [Test]
    public void MangaReadUpdate()
    {
      var newManga = Builder.CreateAcomics();
      var mangaId = newManga.Id;
      var name = newManga.Name;
      var updatedName = name + "2";

      using (var context = Repository.GetEntityContext())
      {
        var manga = context.Get<IManga>().Single(m => m.Id == mangaId);
        Assert.AreEqual(name, manga.Name);
        manga.Name = updatedName;
        context.Save(manga);
      }

      using (var context = Repository.GetEntityContext())
      {
        var manga = context.Get<IManga>().Single(m => m.Id == mangaId);
        Assert.AreEqual(updatedName, manga.Name);
      }
    }

    [Test]
    public void MangaChangeUri()
    {
      var newManga = Builder.CreateAcomics();
      var mangaId = newManga.Id;
      var uri = newManga.Uri;
      var testUri = new Uri(uri, "test");

      using (var context = Repository.GetEntityContext())
      {
        var manga = context.Get<IManga>().Single(m => m.Id == mangaId);
        manga.Uri = testUri;
        context.Save(manga);
        Assert.AreNotEqual(uri, manga.Uri);
      }

      using (var context = Repository.GetEntityContext())
      {
        var manga = context.Get<IManga>().Single(m => m.Id == mangaId);
        manga.Uri = Builder.ReadmangaUri;
        Assert.Catch<SaveValidationException>(() => context.Save(manga));
        Assert.AreEqual(Builder.ReadmangaUri, manga.Uri);
      }

      using (var context = Repository.GetEntityContext())
      {
        var manga = context.Get<IManga>().Single(m => m.Id == mangaId);
        Assert.AreEqual(testUri, manga.Uri);
      }
    }
  }
}
