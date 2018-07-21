using System;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using NUnit.Framework;

namespace Tests.Entities.CRUD
{
  [TestFixture]
  public class MangaHistory : TestClass
  {
    [Test]
    public void MangaHistoryCreateDelete()
    {
      using (var context = Repository.GetEntityContext())
      {
        var manga = Builder.CreateAcomics();
        Builder.CreateMangaHistory(manga);
        var history = manga.Histories.FirstOrDefault();
        var historyId = history.Id;
        var mangaId = manga.Id;
        Assert.AreNotEqual(0, history.Id);
        Assert.AreNotEqual(null, manga.Histories.FirstOrDefault());

        var mangaHistory = context.Get<MangaReader.Core.Manga.MangaHistory>().FirstOrDefault(h => h.Id == historyId);
        Assert.AreNotEqual(null, mangaHistory);

        var mangas = context.Get<IManga>().FirstOrDefault(m => m.Id == mangaId);
        Assert.AreNotEqual(null, mangas);

        Builder.DeleteMangaHistory(manga);
        Builder.DeleteAcomics(manga);

        mangaHistory = context.Get<MangaReader.Core.Manga.MangaHistory>().FirstOrDefault(h => h.Id == historyId);
        Assert.AreEqual(null, mangaHistory);

        mangas = context.Get<IManga>().FirstOrDefault(m => m.Id == mangaId);
        Assert.AreEqual(null, mangas);
      }
    }

    [Test]
    public void MangaHistoryReadUpdate()
    {
      var url = new Uri("http://rutracker.org/forum/viewforum.php?f=52");
      using (var context = Repository.GetEntityContext())
      {
        var manga = Builder.CreateReadmanga();
        Builder.CreateMangaHistory(manga);
        var history = manga.Histories.FirstOrDefault();
        var oldUrl = history.Uri;
        var historyId = history.Id;

        history.Uri = url;
        Assert.AreEqual(url, history.Uri);

        context.Refresh(history);
        Assert.AreEqual(oldUrl, history.Uri);

        var mangaHistory = Repository.GetStateless<MangaReader.Core.Manga.MangaHistory>(historyId);
        Assert.AreEqual(oldUrl, mangaHistory.Uri);

        history.Uri = url;
        context.Save(history);
        Assert.AreEqual(url, history.Uri);

        mangaHistory = Repository.GetStateless<MangaReader.Core.Manga.MangaHistory>(historyId);
        Assert.AreEqual(url, mangaHistory.Uri);

        Builder.DeleteMangaHistory(manga);
        Builder.DeleteReadmanga(manga);
      }
    }
  }
}
