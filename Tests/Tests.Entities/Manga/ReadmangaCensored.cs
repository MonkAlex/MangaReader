using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grouple;
using MangaReader.Core;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture, ReadManga]
  public class ReadmangaCensored : TestClass
  {
    private ReadmangaParser parser = new Grouple.ReadmangaParser();

    // Not censored
    // https://readmanga.live/black_butler_anthology_comic_rainbow_butler/vol1/6
    // Censored
    // https://readmanga.live/school_teacher/vol2/10?mtr=1

    [Test]
    public async Task NotCensoredReadmanga()
    {
      var manga = await Get($"{Grouple.Constants.ReadmangaHost}black_butler_anthology_comic_rainbow_butler/vol1/6").ConfigureAwait(false);
      var chapter = manga.Volumes.Single(v => v.Number == 1).Container.ToList()[0];
      await parser.UpdatePages(chapter).ConfigureAwait(false);
      Assert.IsTrue(chapter.Container.First().ImageLink.IsAbsoluteUri);
    }

    [Test]
    public async Task CensoredReadmanga()
    {
      var manga = await Get($"{Grouple.Constants.ReadmangaHost}school_teacher/vol2/10?mtr=1").ConfigureAwait(false);
      var chapter = manga.Volumes.Single(v => v.Number == 2).Container.ToList()[5];
      await parser.UpdatePages(chapter).ConfigureAwait(false);
      Assert.IsTrue(chapter.Container.First().ImageLink.IsAbsoluteUri);
    }

    [Test]
    public async Task MangaRemovedCopyright()
    {
      var error = string.Empty;

      void OnLogOnLogReceived(LogEventStruct les)
      {
        if (les.Level == "Error")
          error = les.FormattedMessage;
      }

      Log.LogReceived += OnLogOnLogReceived;
      var manga = await Get(@"https://mintmanga.live/in_the_first_grade").ConfigureAwait(false);
      Log.LogReceived -= OnLogOnLogReceived;
      var chapters = manga.Volumes.SelectMany(v => v.Container).ToList();
      Assert.IsTrue(!chapters.Any());
      Assert.AreEqual("Запрещена публикация произведения по копирайту, адрес манги https://mintmanga.live/in_the_first_grade", error);
    }

    private async Task<IManga> Get(string url)
    {
      var manga = await Mangas.Create(new Uri(url)).ConfigureAwait(false);
      await parser.UpdateContent(manga).ConfigureAwait(false);
      return manga;
    }
  }
}
