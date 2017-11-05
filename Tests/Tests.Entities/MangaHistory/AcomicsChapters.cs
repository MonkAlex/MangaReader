using System;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.MangaHistory
{
  [TestFixture]
  public class AcomicsChapters : TestClass
  {
    [Test]
    public void CreateWithHistoryAndGetLastVolume()
    {
      var manga = Builder.CreateAcomics();
      manga.Uri = new Uri("http://acomics.ru/~ma3");

      var volumeUri = new Uri("http://acomics.ru/~ma3/935");
      var chapterUri = new Uri("http://acomics.ru/~ma3/1129");
      manga.Histories.Add(new MangaReader.Core.Manga.MangaHistory(chapterUri));
      manga.Save();

      manga.Refresh();
      manga.Name = Guid.NewGuid().ToString();
      manga.Save();

      var chapter = new Chapter(chapterUri);
      chapter.Container.Add(new MangaPage(new Uri("http://acomics.ru/~ma3/1130"), null, 1));
      var volume = new Volume() { Uri = volumeUri };
      volume.Container.Add(chapter);

      var newChapters = History.GetItemsWithoutHistory(volume);
      Assert.AreEqual(1, newChapters.Count);
    }

    [Test]
    public void CreateWithHistoryAndGetNotLastVolume()
    {
      var manga = Builder.CreateAcomics();
      manga.Uri = new Uri("http://acomics.ru/~ma3");

      var volumeUri = new Uri("http://acomics.ru/~ma3/777");
      var chapterUri = new Uri("http://acomics.ru/~ma3/793");
      manga.Histories.Add(new MangaReader.Core.Manga.MangaHistory(chapterUri));
      manga.Save();

      manga.Refresh();
      manga.Name = Guid.NewGuid().ToString();
      manga.Save();

      var chapter = new Chapter(chapterUri);
      chapter.Container.Add(new MangaPage(new Uri("http://acomics.ru/~ma3/794"), null, 1));
      var volume = new Volume() { Uri = volumeUri };
      volume.Container.Add(chapter);

      var newChapters = History.GetItemsWithoutHistory(volume);
      Assert.AreEqual(1, newChapters.Count);
    }
  }
}
