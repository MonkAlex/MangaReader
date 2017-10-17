using System;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using NUnit.Framework;

namespace Tests.Entities.Mapping
{
  [TestFixture]
  public class Simple : TestClass
  {
    [Test, Order(2)]
    public void AcomicsVolumesChaptersAndPages()
    {
      var mangas = Repository.Get<IManga>().Where(m => m.ServerName == "Strays").ToList();
      foreach (var deleting in mangas)
        deleting.Delete();

      var firstChapterName = "Глава 1. Беспризорница";
      var chapterRenamed = "Test";
      var manga = Builder.CreateAcomics();
      manga.Uri = new Uri("https://acomics.ru/~strays");
      manga.Refresh();
      manga.Parser.UpdateContent(manga);
      manga.Save();

      Assert.AreEqual(3, manga.Volumes.Count);
      Assert.AreEqual(1, manga.Pages.Count);
      Assert.AreEqual(firstChapterName, manga.Volumes.First().Container.First().Name);
      Assert.IsNotEmpty(manga.Volumes.SelectMany(v => v.Container));
      manga.Volumes.Remove(manga.Volumes.Last());
      manga.Pages.Clear();
      manga.Volumes.First().Container.First().Name = chapterRenamed;
      manga.Save();

      Assert.AreEqual(2, manga.Volumes.Count);
      Assert.AreEqual(0, manga.Pages.Count);
      Assert.AreEqual(chapterRenamed, manga.Volumes.First().Container.First().Name);
      Assert.IsNotEmpty(manga.Volumes.SelectMany(v => v.Container));
      manga.Parser.UpdateContent(manga);
      manga.Save();

      Assert.AreEqual(3, manga.Volumes.Count);
      Assert.AreEqual(1, manga.Pages.Count);
      Assert.AreEqual(firstChapterName, manga.Volumes.First().Container.First().Name);
      Assert.IsNotEmpty(manga.Volumes.SelectMany(v => v.Container));
    }
  }
}