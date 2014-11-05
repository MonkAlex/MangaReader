using MangaReader.Manga;
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
      Environment.Initialize();
      Mapping.Environment.SessionFactory = Environment.SessionFactory;


      var history = Builder.CreateMangaHistory();
      var historyId = history.Id;
      var mangaId = history.Manga.Id;
      Assert.AreNotEqual(0, history.Id);
      Assert.AreNotEqual(null, history.Manga);
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

      Builder.DeleteMangaHistory(history);
      Assert.AreEqual(0, history.Id);
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
      var url = "test_url";

      Environment.Initialize();
      Mapping.Environment.SessionFactory = Environment.SessionFactory;


      var history = Builder.CreateMangaHistory();
      var oldUrl = history.Url;
      var historyId = history.Id;

      history.Url = url;
      Assert.AreEqual(url, history.Url);

      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<MangaReader.MangaHistory>(historyId);
        Assert.AreEqual(oldUrl, fromDb.Url);
      }
      history.Update();
      Assert.AreEqual(oldUrl, history.Url);
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<MangaReader.MangaHistory>(historyId);
        Assert.AreEqual(oldUrl, fromDb.Url);
      }

      history.Url = url;
      history.Save();
      Assert.AreEqual(url, history.Url);
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<MangaReader.MangaHistory>(historyId);
        Assert.AreEqual(url, fromDb.Url);
      }

      Builder.DeleteMangaHistory(history);
    }
  }
}
