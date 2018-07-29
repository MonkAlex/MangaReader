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
    public void AddHistoryMultithreadAndSave([Values(true, false)] bool inSession)
    {
      using (inSession ? Repository.GetEntityContext() : null)
      {
        string name;
        using (var context = Repository.GetEntityContext())
        {
          var manga = Builder.CreateReadmanga();
          name = manga.Name;
          context.Save(manga);
          Directory.CreateDirectory(manga.GetAbsoulteFolderPath());
        }
        var manga2 = Builder.CreateReadmanga();

        var exception = Assert.Catch<SaveValidationException>(() =>
        {
          using (var context = Repository.GetEntityContext())
          {
            var manga3 = Builder.CreateReadmanga();
            Directory.CreateDirectory(manga3.GetAbsoulteFolderPath());
            manga3.Name = name;
            AddRandomHistory(manga3);
            Assert.AreEqual(y, manga3.Histories.Count());
            context.Save(manga3);
          }
        });
        Assert.IsAssignableFrom<SaveValidationException>(exception);

        AddRandomHistory(manga2);
        if (inSession)
        {
#warning Сессия, запоротая исключением, запорота навсегда.
          Assert.Catch<SaveValidationException>(() =>
          {
            ResaveManga(manga2);
          });
        }
        else
          ResaveManga(manga2);

        Assert.AreEqual(y, manga2.Histories.Count());
      }
    }

    private static void ResaveManga(MangaReader.Core.Manga.IManga manga)
    {
      using (var context = Repository.GetEntityContext())
      {
        manga = context.Get<MangaReader.Core.Manga.IManga>().Single(m => m.Id == manga.Id);
        context.Save(manga);
      }
    }

    private static void AddRandomHistory(Readmanga manga)
    {
      var x = 100000;
      Parallel.For(0, x, i =>
      {
        manga.Histories.Add(new MangaReader.Core.Manga.MangaHistory(new Uri(string.Format("http://readmanga.me/btoom/vol1/{0}?mature=1", i % y))));
      });
    }
  }
}
