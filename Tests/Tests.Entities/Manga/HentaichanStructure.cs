using System;
using System.Linq;
using System.Threading.Tasks;
using Hentaichan;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class HentaichanStructure : TestClass
  {
    [Test]
    public async Task AddHentaichanMultiPages()
    {
      Login();
      var manga = await GetManga("http://henchan.me/manga/14212-love-and-devil-glava-25.html");
      Unlogin();
      Assert.AreEqual(25, manga.Chapters.Count);
      Assert.IsTrue(manga.HasChapters);
    }

    [Test]
    public async Task AddHentaichanOneChapter()
    {
      var manga = await GetManga("http://henchan.me/manga/15131-chuui-horeru-to-yakui-kara.html");
      Assert.AreEqual(1, manga.Chapters.Count);
      Assert.IsTrue(manga.HasChapters);
    }

    [Test]
    public async Task AddHentaichanSubdomain()
    {
      var manga = await GetManga("http://henchan.me/manga/23083-ponpharse-tokubetsu-hen-chast-1.html");
      Assert.AreEqual(2, manga.Chapters.Count);
      Assert.IsTrue(manga.HasChapters);
    }


    [Test]
    public async Task HentaichanNameParsing()
    {
      // Спецсимвол \
      await TestNameParsing("http://henchan.me/manga/14504-lets-play-lovegames-shall-we-glava-1.html",
        "Let's Play Lovegames, Shall We?");

      // Спецсимвол # и одна глава
      await TestNameParsing("http://henchan.me/manga/15109-exhibitionist-renko-chan.html",
        "#Exhibitionist Renko-chan");

      // Символ звездочки *
      await TestNameParsing("http://henchan.me/manga/15131-chuui-horeru-to-yakui-kara.html",
        "*Chuui* Horeru to Yakui kara");

      // Символ /
      await TestNameParsing("http://henchan.me/manga/10535-blush-dc.-glava-1.html",
        "/Blush-DC.");

      // На всякий случай
      await TestNameParsing("http://henchan.me/manga/23083-ponpharse-tokubetsu-hen-chast-1.html",
        "Ponpharse - Tokubetsu Hen");

      // Манга требующая регистрации для просмотра
      await TestNameParsing("http://henchan.me/manga/14212-love-and-devil-glava-25.html",
        "Love and Devil");
    }

    private async Task TestNameParsing(string uri, string name)
    {
      ConfigStorage.Instance.AppConfig.Language = Languages.English;
      var manga = await GetManga(uri);
      Assert.AreEqual(name, manga.Name);
    }

    private async Task<Hentaichan.Hentaichan> GetManga(string url)
    {
      var manga = await Mangas.CreateFromWeb(new Uri(url)) as Hentaichan.Hentaichan;
      return manga;
    }

    private void Login()
    {
      using (var context = Repository.GetEntityContext())
      {
        var userId = "235332";
        var setting = ConfigStorage.GetPlugin<Hentaichan.Hentaichan>().GetSettings();
        var login = setting.Login as Hentaichan.HentaichanLogin;
        if (login.UserId != userId)
        {
          login.UserId = userId;
          login.PasswordHash = "0578caacc02411f8c9a1a0af31b3befa";
          login.IsLogined = true;
          context.Save(setting);
        }
      }
    }

    private void Unlogin()
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Hentaichan.Hentaichan>().GetSettings();
        var login = setting.Login as Hentaichan.HentaichanLogin;
        login.UserId = "";
        login.PasswordHash = "";
        login.IsLogined = false;
        context.Save(setting);
      }
    }
  }
}