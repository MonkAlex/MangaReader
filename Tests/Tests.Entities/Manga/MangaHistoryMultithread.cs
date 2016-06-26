using System;
using System.Threading.Tasks;
using MangaReader.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Manga
{
  [TestClass]
  public class MangaHistoryMultithread
  {
    [TestMethod]
    public void AddHistoryMultithread()
    {
      var manga = Builder.CreateReadmanga();
      Parallel.For(0, 100, i =>
      {
        manga.AddHistory(new Uri(string.Format("http://readmanga.me/btoom/vol1/{0}?mature=1", i)));
      });
      Assert.AreEqual(10000, manga.Histories.Count);
    }
  }
}
