using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grouple;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
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
    public async Task AddHistoryMultithread()
    {
      var manga = await Builder.CreateReadmanga().ConfigureAwait(false);
      AddRandomHistory(manga);
      Assert.AreEqual(y, manga.Histories.Count());
    }

    /// <summary>
    /// #102 - после исключения в beforesave история одной манги мешает создавать историю другой манги.
    /// </summary>
    [Test]
    public async Task AddHistoryMultithreadAndSave([Values(true, false)] bool inSession)
    {
      using (inSession ? Repository.GetEntityContext() : null)
      {
        string name;
        using (var context = Repository.GetEntityContext())
        {
          var manga = await Builder.CreateReadmanga().ConfigureAwait(false);
          name = manga.Name;
          await context.Save(manga).ConfigureAwait(false);
          Directory.CreateDirectory(manga.GetAbsoluteFolderPath());
        }
        var manga2 = await Builder.CreateReadmanga().ConfigureAwait(false);

        var exception = Assert.CatchAsync<MangaSaveValidationException>(async () =>
        {
          using (var context = Repository.GetEntityContext())
          {
            var manga3 = await Builder.CreateReadmanga().ConfigureAwait(false);
            Directory.CreateDirectory(manga3.GetAbsoluteFolderPath());
            manga3.Name = name;
            AddRandomHistory(manga3);
            Assert.AreEqual(y, manga3.Histories.Count());
            await context.Save(manga3).ConfigureAwait(false);
          }
        });
        Assert.IsAssignableFrom<MangaSaveValidationException>(exception);

        AddRandomHistory(manga2);
        if (inSession)
        {
#warning Сессия, запоротая исключением, запорота навсегда.
          Assert.CatchAsync<MangaSaveValidationException>(async () =>
          {
            await ResaveManga(manga2).ConfigureAwait(false);
          });
        }
        else
          await ResaveManga(manga2).ConfigureAwait(false);

        Assert.AreEqual(y, manga2.Histories.Count());
      }
    }

    private static async Task ResaveManga(MangaReader.Core.Manga.IManga manga)
    {
      using (var context = Repository.GetEntityContext())
      {
        manga = await context.Get<IManga>().SingleAsync(m => m.Id == manga.Id).ConfigureAwait(false);
        await context.Save(manga).ConfigureAwait(false);
      }
    }

    private static void AddRandomHistory(Readmanga manga)
    {
      var x = 100000;
      Parallel.For(0, x, i =>
      {
        manga.Histories.Add(new MangaReader.Core.Manga.MangaHistory(new Uri(string.Format("https://readmanga.io/btoom/vol1/{0}?mtr=1", i % y))));
      });
    }
  }
}
