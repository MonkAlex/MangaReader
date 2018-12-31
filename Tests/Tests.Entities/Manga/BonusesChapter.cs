using System;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
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
      var chapters = manga.Volumes.SelectMany(v => v.Container).ToList();
      Assert.AreEqual(1, chapters.Count(c => c.Number == 170));
      Assert.AreEqual(1, chapters.Count(c => c.Number == 170.1));
    }

    [Test]
    public void ReadmangaBonus()
    {
      var manga = Mangas.CreateFromWeb(new Uri("http://readmanga.me/animal_country"));
      manga.Parser.UpdateContent(manga);
      var chapters = manga.Volumes.SelectMany(v => v.Container).OfType<Grouple.GroupleChapter>();
      Assert.AreEqual(1, chapters.Count(c => c.VolumeNumber == 14 && c.Number == 54));
      Assert.AreEqual(1, chapters.Count(c => c.VolumeNumber == 6 && c.Number == 22));
    }

    [Test]
    public void MintmangaBonus()
    {
      using (var context = Repository.GetEntityContext())
      {
        var uri = new Uri("http://mintmanga.com/haruka_na_receive");
        var toRemove = context.Get<IManga>().Where(m => m.Uri == uri).ToList();
        foreach (var remove in toRemove)
          context.Delete(remove);
        var manga = Mangas.CreateFromWeb(uri);
        manga.Parser.UpdateContent(manga);
        var chapters = manga.Volumes.SelectMany(v => v.Container).OfType<Grouple.GroupleChapter>();
        Assert.AreEqual(1, chapters.Count(c => c.VolumeNumber == 1 && c.Number == 0));
      }
    }
  }
}