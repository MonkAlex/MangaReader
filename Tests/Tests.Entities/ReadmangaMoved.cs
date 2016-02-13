using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader;
using MangaReader.Manga;
using MangaReader.Manga.Grouple;
using MangaReader.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Linq;
using Chapter = MangaReader.Manga.Grouple.Chapter;

namespace Tests.Entities
{
  [TestClass]
  public class ReadmangaMoved
  {
    [TestMethod]
    public void CreateWithHistoryAndMove()
    {
      foreach (var remove in Environment.Session.Query<Mangas>().Where(m => m.ServerName.Contains("btooom")).ToList())
        MangaReader.Services.Library.Remove(remove);

      var manga = Builder.CreateReadmanga();
      manga.Uri = new Uri("http://readmanga.me/btoom");
      manga.Histories.Add(new MangaHistory(new Uri("http://readmanga.me/btoom/vol1/1?mature=1")));
      manga.Save();

      manga = Environment.Session.Get<Readmanga>(manga.Id);
      manga.Refresh();
      manga.Save();
      var chapters = new List<Chapter> { new Chapter(new Uri("http://mintmanga.com/btooom_/vol1/1?mature=1")) };
      var chartersNotInHistory = MangaReader.Services.History.GetItemsWithoutHistory(chapters);
      Assert.AreEqual(0, chartersNotInHistory.Count);
    }
  }
}
