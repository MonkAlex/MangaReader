using System.Linq;
using System.Threading.Tasks;
using Grouple;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class MangaLocalName : TestClass
  {
    [Test]
    public async Task SetLocalName()
    {
      using (var context = Repository.GetEntityContext())
      {
        var mangaLocalName = "LocalName";
        var manga = await Builder.CreateReadmanga().ConfigureAwait(false);
        var serverName = manga.Name;
        Assert.AreEqual(serverName, manga.Name);
        Assert.AreEqual(serverName, manga.ServerName);
        Assert.AreEqual(serverName, manga.LocalName);

        manga.IsNameChanged = true;
        manga.Name = mangaLocalName;
        Assert.AreEqual(mangaLocalName, manga.Name);
        Assert.AreEqual(mangaLocalName, manga.LocalName);
        Assert.AreEqual(serverName, manga.ServerName);

        await context.Save(manga).ConfigureAwait(false);
        manga = await context.Get<Readmanga>().SingleAsync(m => m.Id == manga.Id).ConfigureAwait(false);
        Assert.AreEqual(mangaLocalName, manga.Name);
        Assert.AreEqual(mangaLocalName, manga.LocalName);
        Assert.AreEqual(serverName, manga.ServerName);

        manga.IsNameChanged = false;
        Assert.AreEqual(serverName, manga.Name);
        Assert.AreEqual(mangaLocalName, manga.LocalName);
        Assert.AreEqual(serverName, manga.ServerName);

        await context.Save(manga).ConfigureAwait(false);
        manga = await context.Get<Readmanga>().SingleAsync(m => m.Id == manga.Id).ConfigureAwait(false);
        Assert.AreEqual(serverName, manga.Name);
        Assert.AreEqual(mangaLocalName, manga.LocalName);
        Assert.AreEqual(serverName, manga.ServerName);
      }
    }
  }
}