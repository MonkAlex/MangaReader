using System;
using System.Linq;
using System.Threading.Tasks;
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
      var x = 100000;
      var y = 100;
      Parallel.For(0, x, i =>
      {
        manga.AddHistory(new Uri(string.Format("http://readmanga.me/btoom/vol1/{0}?mature=1", i % y)));
      });
      Assert.AreEqual(y, manga.Histories.Count());
    }
  }
}
