using System;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.MangaHistory
{
  [TestFixture]
  public class AcomicsChapters : TestClass
  {
    [Test]
    public async Task CreateWithHistoryAndGetLastVolume()
    {
      using (var context = Repository.GetEntityContext())
      {
        var manga = Builder.CreateAcomics();
        manga.Uri = new Uri("https://acomics.ru/~ma3");

        var volumeUri = new Uri("https://acomics.ru/~ma3/935");
        var chapterUri = new Uri("https://acomics.ru/~ma3/1129");
        manga.Histories.Add(new MangaReader.Core.Manga.MangaHistory(chapterUri));
        context.Save(manga);

        await manga.Refresh();
        manga.Name = Guid.NewGuid().ToString();
        context.Save(manga);

        var chapter = new Chapter(chapterUri, string.Empty);
        chapter.Container.Add(new MangaPage(new Uri("https://acomics.ru/~ma3/1130"), null, 1));
        var volume = new Volume() { Uri = volumeUri };
        volume.Container.Add(chapter);

        var newChapters = History.GetItemsWithoutHistory(volume);
        Assert.AreEqual(1, newChapters.Count);
      }
    }

    [Test]
    public async Task CreateWithHistoryAndGetNotLastVolume()
    {
      using (var context = Repository.GetEntityContext())
      {
        var manga = Builder.CreateAcomics();
        manga.Uri = new Uri("https://acomics.ru/~ma3");

        var volumeUri = new Uri("https://acomics.ru/~ma3/777");
        var chapterUri = new Uri("https://acomics.ru/~ma3/793");
        manga.Histories.Add(new MangaReader.Core.Manga.MangaHistory(chapterUri));
        context.Save(manga);

        await manga.Refresh();
        manga.Name = Guid.NewGuid().ToString();
        context.Save(manga);

        var chapter = new Chapter(chapterUri, string.Empty);
        chapter.Container.Add(new MangaPage(new Uri("https://acomics.ru/~ma3/794"), null, 1));
        var volume = new Volume() { Uri = volumeUri };
        volume.Container.Add(chapter);

        var newChapters = History.GetItemsWithoutHistory(volume);
        Assert.AreEqual(1, newChapters.Count);
      }
    }
  }
}
