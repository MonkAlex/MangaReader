using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.Mapping
{
  [TestFixture]
  public class Simple : TestClass
  {
    [Test, Order(2)]
    public async Task AcomicsVolumesChaptersAndPages()
    {
      using (var context = Repository.GetEntityContext())
      {
        var mangas = await context.Get<IManga>().Where(m => m.ServerName == "Strays").ToListAsync().ConfigureAwait(false);
        foreach (var deleting in mangas)
          await context.Delete(deleting).ConfigureAwait(false);

        // Init
        var firstChapterName = "Глава 1. Беспризорница";
        var chapterRenamed = "Test";
        var manga = await Builder.CreateAcomics().ConfigureAwait(false);
        manga.Uri = new Uri("https://acomics.ru/~strays");
        await manga.Refresh().ConfigureAwait(false);
        await manga.Parser.UpdateContent(manga).ConfigureAwait(false);
        await context.Save(manga).ConfigureAwait(false);
        Assert.AreEqual(3, manga.Volumes.Count);
        Assert.AreEqual(1, manga.Pages.Count);
        Assert.AreEqual(firstChapterName, manga.Volumes.First().Container.First().Name);
        Assert.IsNotEmpty(manga.Volumes.SelectMany(v => v.Container));

        // Удаление тома.
        manga.Volumes.Remove(manga.Volumes.Last());

        // Удаление страниц (одной).
        manga.Pages.Clear();

        // Первой попавшейся главе меняем имя.
        manga.Volumes.First().Container.First().Name = chapterRenamed;
        await context.Save(manga).ConfigureAwait(false);
        Assert.AreEqual(2, manga.Volumes.Count);
        Assert.AreEqual(0, manga.Pages.Count);
        Assert.AreEqual(chapterRenamed, manga.Volumes.First().Container.First().Name);
        Assert.IsNotEmpty(manga.Volumes.SelectMany(v => v.Container));

        // Перечитываем состояние с сайта.
        await manga.Parser.UpdateContent(manga).ConfigureAwait(false);
        await context.Save(manga).ConfigureAwait(false);
        Assert.AreEqual(3, manga.Volumes.Count);
        Assert.AreEqual(1, manga.Pages.Count);
        Assert.AreEqual(firstChapterName, manga.Volumes.First().Container.First().Name);
        Assert.IsNotEmpty(manga.Volumes.SelectMany(v => v.Container));
      }
    }
  }
}