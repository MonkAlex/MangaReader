using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Core;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class ReadmangaCensored : TestClass
  {
    // Not censored
    // http://readmanga.me/black_butler_anthology_comic_dj____rainbow_butler/vol1/6
    // Censored
    // http://readmanga.me/school_teacher/vol2/10?mature=1

    [Test]
    public void NotCensoredReadmanga()
    {
      var manga = Get(@"http://readmanga.me/black_butler_anthology_comic_dj____rainbow_butler/vol1/6");
      var chapter = manga.Volumes.Single(v => v.Number == 1).Container.ToList()[0];
      Grouple.Parser.UpdatePages(chapter as Grouple.GroupleChapter);
      Assert.IsTrue(chapter.Container.First().ImageLink.IsAbsoluteUri);
    }

    [Test]
    public void CensoredReadmanga()
    {
      var manga = Get(@"http://readmanga.me/school_teacher/vol2/10?mature=1");
      var chapter = manga.Volumes.Single(v => v.Number == 2).Container.ToList()[5];
      Grouple.Parser.UpdatePages(chapter as Grouple.GroupleChapter);
      Assert.IsTrue(chapter.Container.First().ImageLink.IsAbsoluteUri);
    }

    [Test]
    public void MangaRemovedCopyright()
    {
      var error = string.Empty;

      void OnLogOnLogReceived(LogEventStruct les)
      {
        if (les.Level == "Error")
          error = les.FormattedMessage;
      }

      Log.LogReceived += OnLogOnLogReceived;
      var manga = Get(@"http://mintmanga.com/in_the_first_grade");
      Log.LogReceived -= OnLogOnLogReceived;
      var chapters = manga.Volumes.SelectMany(v => v.Container).ToList();
      Assert.IsTrue(!chapters.Any());
      Assert.AreEqual("Запрещена публикация произведения по копирайту, адрес манги http://mintmanga.com/in_the_first_grade", error);
    }

    private IManga Get(string url)
    {
      var manga = Mangas.Create(new Uri(url));
      new Grouple.Parser().UpdateContent(manga);
      return manga;
    }
  }
}
