using System;
using MangaReader.Manga;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.CRUD
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

      var fromDb = Environment.Session.Get<Mangas>(mangaId);
      Assert.AreNotEqual(null, fromDb);

      Builder.DeleteReadmanga(newManga);
      Assert.AreEqual(0, newManga.Id);

      fromDb = Environment.Session.Get<Mangas>(mangaId);
      Assert.AreEqual(null, fromDb);
    }

    [TestMethod]
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

      var fromDb = Environment.Session.Get<Mangas>(mangaId);
      Assert.AreEqual(oldUrl, fromDb.Uri);

      newManga.Uri = url;
      newManga.Save();
      Assert.AreEqual(url, newManga.Uri);

      fromDb = Environment.Session.Get<Mangas>(mangaId);
      Assert.AreEqual(url, fromDb.Uri);

      Builder.DeleteAcomics(newManga);
    }
  }
}
