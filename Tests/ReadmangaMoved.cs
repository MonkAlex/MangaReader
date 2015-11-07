using System;
using System.Collections.Generic;
using MangaReader.Manga.Grouple;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MangaReader.Tests
{
  [TestClass]
  public class ReadmangaMoved
  {
    [TestMethod]
    public void CreateWithHistoryAndMove()
    {
      var manga = Builder.CreateReadmanga();
      manga.Uri = new Uri("http://readmanga.me/btoom");
      manga.Histories.Add(new MangaHistory(new Uri("http://readmanga.me/btoom/vol1/1?mature=1")));
      manga.Save();

      manga = Environment.Session.Get<Readmanga>(manga.Id);
      manga.Refresh();
      manga.Save();
      var chapters = new List<Chapter> { new Chapter(new Uri("http://adultmanga.ru/btooom_/vol1/1?mature=1")) };
      var chartersNotInHistory = Services.History.GetItemsWithoutHistory(chapters);
      Assert.AreEqual(0, chartersNotInHistory.Count);
    }
  }
}
