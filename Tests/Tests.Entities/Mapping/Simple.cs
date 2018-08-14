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
      using (var context = Repository.GetEntityContext())
      {
        var mangas = context.Get<IManga>().Where(m => m.ServerName == "Strays");
        foreach (var deleting in mangas)
          context.Delete(deleting);

        // Init
        var firstChapterName = "Глава 1. Беспризорница";
        var chapterRenamed = "Test";
        var manga = Builder.CreateAcomics();
        manga.Uri = new Uri("https://acomics.ru/~strays");
        manga.Refresh();
        manga.Parser.UpdateContent(manga);
        context.Save(manga);
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
        context.Save(manga);
        Assert.AreEqual(2, manga.Volumes.Count);
        Assert.AreEqual(0, manga.Pages.Count);
        Assert.AreEqual(chapterRenamed, manga.Volumes.First().Container.First().Name);
        Assert.IsNotEmpty(manga.Volumes.SelectMany(v => v.Container));

        // Перечитываем состояние с сайта.
        manga.Parser.UpdateContent(manga);
        context.Save(manga);
        Assert.AreEqual(3, manga.Volumes.Count);
        Assert.AreEqual(1, manga.Pages.Count);
        Assert.AreEqual(firstChapterName, manga.Volumes.First().Container.First().Name);
        Assert.IsNotEmpty(manga.Volumes.SelectMany(v => v.Container));
      }
    }
  }
}