using System;
using System.Linq;
using MangaReader.Manga;
using MangaReader.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Environment = Tests.Environment;

namespace MangaReader.Tests.CRUD
{
  [TestClass]
  public class MangaHistory
  {
    [TestMethod]
    public void MangaHistoryCreateDelete()
    {
      var manga = Builder.CreateAcomics();
      Builder.CreateMangaHistory(manga);
      var history = manga.Histories.FirstOrDefault();
      var historyId = history.Id;
      var mangaId = manga.Id;
      Assert.AreNotEqual(0, history.Id);
      Assert.AreNotEqual(null, manga.Histories.FirstOrDefault());
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<MangaReader.MangaHistory>(historyId);
        Assert.AreNotEqual(null, fromDb);
      }
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<Mangas>(mangaId);
        Assert.AreNotEqual(null, fromDb);
      }

      Builder.DeleteMangaHistory(manga);
      Builder.DeleteAcomics(manga);
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<MangaReader.MangaHistory>(historyId);
        Assert.AreEqual(null, fromDb);
      }
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<Mangas>(mangaId);
        Assert.AreEqual(null, fromDb);
      }
    }

    [TestMethod]
    public void MangaHistoryReadUpdate()
    {
      var url = new Uri("http://rutracker.org/forum/viewforum.php?f=52");
      var manga = Builder.CreateReadmanga();
      Builder.CreateMangaHistory(manga);
      var history = manga.Histories.FirstOrDefault();
      var oldUrl = history.Uri;
      var historyId = history.Id;

      history.Uri = url;
      Assert.AreEqual(url, history.Uri);

      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<MangaReader.MangaHistory>(historyId);
        Assert.AreEqual(oldUrl, fromDb.Uri);
      }
      history.Update();
      Assert.AreEqual(oldUrl, history.Uri);
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<MangaReader.MangaHistory>(historyId);
        Assert.AreEqual(oldUrl, fromDb.Uri);
      }

      history.Uri = url;
      history.Save();
      Assert.AreEqual(url, history.Uri);
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<MangaReader.MangaHistory>(historyId);
        Assert.AreEqual(url, fromDb.Uri);
      }

      Builder.DeleteMangaHistory(manga);
      Builder.DeleteReadmanga(manga);
    }
  }
}
