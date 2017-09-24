using System;
using System.Linq;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class BonusesChapter : TestClass
  {
    [Test]
    public void MangachanBonus()
    {
      var manga = Mangas.CreateFromWeb(new Uri("http://mangachan.me/manga/5335-the-breaker-new-waves.html"));
      manga.Parser.UpdateContent(manga);
      var chapters = manga.Volumes.SelectMany(v => v.Container);
      Assert.AreEqual(1, chapters.Count(c => c.Number == 170));
      Assert.AreEqual(1, chapters.Count(c => c.Number == 170.1));
    }

    [Test]
    public void ReadmangaBonus()
    {
      var manga = Mangas.CreateFromWeb(new Uri("http://readmanga.me/animal_country"));
      manga.Parser.UpdateContent(manga);
      var chapters = manga.Volumes.SelectMany(v => v.Container).OfType<Grouple.Chapter>();
      Assert.AreEqual(1, chapters.Count(c => c.Volume == 14 && c.Number == 54));
      Assert.AreEqual(1, chapters.Count(c => c.Volume == 6 && c.Number == 22));
    }

    [Test]
    public void MintmangaBonus()
    {
      var manga = Mangas.CreateFromWeb(new Uri("http://mintmanga.com/haruka_na_receive"));
      manga.Parser.UpdateContent(manga);
      var chapters = manga.Volumes.SelectMany(v => v.Container).OfType<Grouple.Chapter>();
      Assert.AreEqual(1, chapters.Count(c => c.Volume == 1 && c.Number == 0));
    }
  }
}