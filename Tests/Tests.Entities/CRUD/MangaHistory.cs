using System;
using System.Linq;
using MangaReader.Core.Manga;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.CRUD
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

      var mangaHistory = Environment.Session.Get<MangaReader.Core.Manga.MangaHistory>(historyId);
      Assert.AreNotEqual(null, mangaHistory);

      var mangas = Environment.Session.Get<Mangas>(mangaId);
      Assert.AreNotEqual(null, mangas);

      Builder.DeleteMangaHistory(manga);
      Builder.DeleteAcomics(manga);

      mangaHistory = Environment.Session.Get<MangaReader.Core.Manga.MangaHistory>(historyId);
      Assert.AreEqual(null, mangaHistory);

      mangas = Environment.Session.Get<Mangas>(mangaId);
      Assert.AreEqual(null, mangas);
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

      history.Update();
      Assert.AreEqual(oldUrl, history.Uri);

      var mangaHistory = Environment.Session.Get<MangaReader.Core.Manga.MangaHistory>(historyId);
      Assert.AreEqual(oldUrl, mangaHistory.Uri);

      history.Uri = url;
      history.Save();
      Assert.AreEqual(url, history.Uri);

      mangaHistory = Environment.Session.Get<MangaReader.Core.Manga.MangaHistory>(historyId);
      Assert.AreEqual(url, mangaHistory.Uri);

      Builder.DeleteMangaHistory(manga);
      Builder.DeleteReadmanga(manga);
    }
  }
}
