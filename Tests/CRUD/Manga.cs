using System;
using MangaReader.Manga;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Environment = Tests.Environment;

namespace MangaReader.Tests.CRUD
{
  [TestClass]
  public class Manga
  {
    [TestMethod]
    public void MangaCreateDelete()
    {
      var newManga = Builder.CreateReadmanga();
      var mangaId = newManga.Id;
      Assert.AreNotEqual(0, newManga.Id);
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<Mangas>(mangaId);
        Assert.AreNotEqual(null, fromDb);
      }

      Builder.DeleteReadmanga(newManga);
      Assert.AreEqual(0, newManga.Id);
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<Mangas>(mangaId);
        Assert.AreEqual(null, fromDb);
      }
    }

    [TestMethod]
    public void MangaReadUpdate()
    {
      var url = new Uri("test_url");
      var newManga = Builder.CreateAcomics();
      var oldUrl = newManga.Uri;
      var mangaId = newManga.Id;

      newManga.Uri = url;
      Assert.AreEqual(url, newManga.Uri);

      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<Mangas>(mangaId);
        Assert.AreEqual(oldUrl, fromDb.Uri);
      }
      newManga.Update();
      Assert.AreEqual(oldUrl, newManga.Uri);
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<Mangas>(mangaId);
        Assert.AreEqual(oldUrl, fromDb.Uri);
      }

      newManga.Uri = url;
      newManga.Save();
      Assert.AreEqual(url, newManga.Uri);
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<Mangas>(mangaId);
        Assert.AreEqual(url, fromDb.Uri);
      }

      Builder.DeleteAcomics(newManga);
    }
  }
}
