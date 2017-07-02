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

      var fromDb = Repository.Get<IManga>(mangaId);
      Assert.AreNotEqual(null, fromDb);

      Builder.DeleteReadmanga(newManga);
      Assert.AreEqual(0, newManga.Id);

      fromDb = Repository.Get<IManga>(mangaId);
      Assert.AreEqual(null, fromDb);
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

      newManga.Update();
      Assert.AreEqual(oldUrl, newManga.Uri);

      var fromDb = Repository.Get<IManga>(mangaId);
      Assert.AreEqual(oldUrl, fromDb.Uri);

      newManga.Uri = url;
      newManga.Save();
      Assert.AreEqual(url, newManga.Uri);

      fromDb = Repository.Get<IManga>(mangaId);
      Assert.AreEqual(url, fromDb.Uri);

      Builder.DeleteAcomics(newManga);
    }
  }
}
