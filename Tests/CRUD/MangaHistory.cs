using System.Linq;
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
      Mapping.Environment.Session = Environment.Session;

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
      var url = "test_url";

      Environment.Initialize();
      Mapping.Environment.SessionFactory = Environment.SessionFactory;


      var manga = Builder.CreateReadmanga();
      Builder.CreateMangaHistory(manga);
      var history = manga.Histories.FirstOrDefault();
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

      Builder.DeleteMangaHistory(manga);
      Builder.DeleteReadmanga(manga);
    }
  }
}
