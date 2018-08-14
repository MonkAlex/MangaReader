using System.Linq;
using MangaReader.Core.NHibernate;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class MangaLocalName : TestClass
  {
    [Test]
    public void SetLocalName()
    {
      using (var context = Repository.GetEntityContext())
      {
        var mangaLocalName = "LocalName";
        var manga = Builder.CreateReadmanga();
        var serverName = manga.Name;
        Assert.AreEqual(serverName, manga.Name);
        Assert.AreEqual(serverName, manga.ServerName);
        Assert.AreEqual(serverName, manga.LocalName);

        manga.IsNameChanged = true;
        manga.Name = mangaLocalName;
        Assert.AreEqual(mangaLocalName, manga.Name);
        Assert.AreEqual(mangaLocalName, manga.LocalName);
        Assert.AreEqual(serverName, manga.ServerName);

        context.Save(manga);
        manga = context.Get<Grouple.Readmanga>().Single(m => m.Id == manga.Id);
        Assert.AreEqual(mangaLocalName, manga.Name);
        Assert.AreEqual(mangaLocalName, manga.LocalName);
        Assert.AreEqual(serverName, manga.ServerName);

        manga.IsNameChanged = false;
        Assert.AreEqual(serverName, manga.Name);
        Assert.AreEqual(mangaLocalName, manga.LocalName);
        Assert.AreEqual(serverName, manga.ServerName);

        context.Save(manga);
        manga = context.Get<Grouple.Readmanga>().Single(m => m.Id == manga.Id);
        Assert.AreEqual(serverName, manga.Name);
        Assert.AreEqual(mangaLocalName, manga.LocalName);
        Assert.AreEqual(serverName, manga.ServerName);
      }
    }
  }
}