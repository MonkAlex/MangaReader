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

      manga.Save();
      manga = Repository.GetStateless<Grouple.Readmanga>(manga.Id);
      Assert.AreEqual(mangaLocalName, manga.Name);
      Assert.AreEqual(mangaLocalName, manga.LocalName);
      Assert.AreEqual(serverName, manga.ServerName);

      manga.IsNameChanged = false;
      Assert.AreEqual(serverName, manga.Name);
      Assert.AreEqual(mangaLocalName, manga.LocalName);
      Assert.AreEqual(serverName, manga.ServerName);

      manga.Save();
      manga = Repository.GetStateless<Grouple.Readmanga>(manga.Id);
      Assert.AreEqual(serverName, manga.Name);
      Assert.AreEqual(mangaLocalName, manga.LocalName);
      Assert.AreEqual(serverName, manga.ServerName);
    }
  }
}