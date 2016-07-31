using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Linq;

namespace Tests.Entities.Manga
{
  [TestClass]
  public class ReadmangaMoved
  {
    [TestMethod]
    public void CreateWithHistoryAndMove()
    {
      foreach (var remove in Environment.Session.Query<Mangas>().Where(m => m.ServerName.Contains("btooom")).ToList())
        MangaReader.Core.Services.Library.Remove(remove);

      var manga = Builder.CreateReadmanga();
      manga.Uri = new Uri("http://readmanga.me/btoom");
      manga.AddHistory(new Uri("http://readmanga.me/btoom/vol1/1?mature=1"));
      manga.Save();

      manga = Environment.Session.Get<Grouple.Readmanga>(manga.Id);
      manga.Refresh();
      manga.Save();
      var chapters = new Volume() { Chapters = new List<MangaReader.Core.Manga.Chapter>
        { new Chapter(new Uri("http://mintmanga.com/btooom_/vol1/1?mature=1"))}};
      var chartersNotInHistory = History.GetItemsWithoutHistory(chapters);
      Assert.AreEqual(0, chartersNotInHistory.Count);
    }
  }
}
