using System;
using System.Linq;
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
      var url = new Uri("http://rutracker.org/forum/viewforum.php?f=52");
      var newManga = Builder.CreateAcomics();
      var oldUrl = newManga.Uri;
      var mangaId = newManga.Id;

      newManga.Uri = url;
      Assert.AreEqual(url, newManga.Uri);

      using (var context = Repository.GetEntityContext())
      {
        context.Refresh(newManga);
        Assert.AreEqual(oldUrl, newManga.Uri);

        var fromDb = context.Get<IManga>().FirstOrDefault(m => m.Id == mangaId);
        Assert.AreEqual(oldUrl, fromDb.Uri);

        newManga.Uri = url;
        context.Save(newManga);
      }
      Assert.AreEqual(url, newManga.Uri);

      using (var context = Repository.GetEntityContext())
      {
        var fromDb = context.Get<IManga>().FirstOrDefault(m => m.Id == mangaId);
        Assert.AreEqual(url, fromDb.Uri);
      }

      Builder.DeleteAcomics(newManga);
    }
  }
}
