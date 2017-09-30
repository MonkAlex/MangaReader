using System;
using System.Linq;
using System.Threading.Tasks;
using Grouple;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class MangaHistoryMultithread : TestClass
  {
    const int y = 100;
    [Test]
    public void AddHistoryMultithread()
    {
      var manga = Builder.CreateReadmanga();
      AddRandomHistory(manga);
      Assert.AreEqual(y, manga.Histories.Count());
    }

    [Test]
    public void AddHistoryMultithreadAndSave()
    {
      var manga = Builder.CreateReadmanga();
      var name = manga.Name;
      manga.Save();

      var exception = Assert.Catch(() =>
      {
        manga = Builder.CreateReadmanga();
        manga.Name = name;
        AddRandomHistory(manga);
        manga.Save();
      });
      Assert.NotNull(exception);

      manga = Builder.CreateReadmanga();
      AddRandomHistory(manga);
      manga.Save();
    }

    private static void AddRandomHistory(Readmanga manga)
    {
      var x = 100000;
      Parallel.For(0, x, i => { manga.AddHistory(new Uri(string.Format("http://readmanga.me/btoom/vol1/{0}?mature=1", i % y))); });
    }
  }
}
