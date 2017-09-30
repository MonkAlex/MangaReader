using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grouple;
using MangaReader.Core.Exception;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
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

    /// <summary>
    /// #102 - после исключения в beforesave история одной манги мешает создавать историю другой манги.
    /// </summary>
    [Test]
    public void AddHistoryMultithreadAndSave()
    {
      var manga = Builder.CreateReadmanga();
      var name = manga.Name;
      manga.Save();
      Directory.CreateDirectory(manga.GetAbsoulteFolderPath());
      var manga2 = Builder.CreateReadmanga();

      var exception = Assert.Catch<TargetInvocationException>(() =>
      {
        var manga3 = Builder.CreateReadmanga();
        Directory.CreateDirectory(manga3.GetAbsoulteFolderPath());
        manga3.Name = name;
        AddRandomHistory(manga3);
        Assert.AreEqual(y, manga3.Histories.Count());
        manga3.Save();
      });
      Assert.IsAssignableFrom<SaveValidationException>(exception.InnerException);

      AddRandomHistory(manga2);
      manga2.Save();
      Assert.AreEqual(y, manga2.Histories.Count());
      Assert.AreEqual(y, Repository.Get<MangaReader.Core.Manga.MangaHistory>().Count());
    }

    private static void AddRandomHistory(Readmanga manga)
    {
      var x = 100000;
      Parallel.For(0, x, i => { manga.AddHistory(new Uri(string.Format("http://readmanga.me/btoom/vol1/{0}?mature=1", i % y))); });
    }
  }
}
