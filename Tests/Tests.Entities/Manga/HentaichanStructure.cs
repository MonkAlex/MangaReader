using System;
using System.Linq;
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
    public void AddHentaichanMultiPages()
    {
      Login();
      var manga = GetManga("http://hentai-chan.me/manga/14212-love-and-devil-glava-25.html");
      Unlogin();
      Assert.AreEqual(25, manga.Chapters.Count);
      Assert.IsTrue(manga.HasChapters);
    }

    [Test]
    public void AddHentaichanOneChapter()
    {
      var manga = GetManga("http://hentai-chan.me/manga/15131-chuui-horeru-to-yakui-kara.html");
      Assert.AreEqual(1, manga.Chapters.Count);
      Assert.IsTrue(manga.HasChapters);
    }

    [Test]
    public void AddHentaichanSubdomain()
    {
      var manga = GetManga("http://hentai-chan.me/manga/23083-ponpharse-tokubetsu-hen-chast-1.html");
      Assert.AreEqual(2, manga.Chapters.Count);
      Assert.IsTrue(manga.HasChapters);
    }


    [Test]
    public void HentaichanNameParsing()
    {
      // Спецсимвол \
      TestNameParsing("http://hentai-chan.me/manga/14504-lets-play-lovegames-shall-we-glava-1.html",
        "Let's Play Lovegames, Shall We?");

      // Спецсимвол # и одна глава
      TestNameParsing("http://hentai-chan.me/manga/15109-exhibitionist-renko-chan.html",
        "#Exhibitionist Renko-chan");

      // Символ звездочки *
      TestNameParsing("http://hentai-chan.me/manga/15131-chuui-horeru-to-yakui-kara.html",
        "*Chuui* Horeru to Yakui kara");

      // Символ /
      TestNameParsing("http://hentai-chan.me/manga/10535-blush-dc.-glava-1.html",
        "/Blush-DC.");

      // На всякий случай
      TestNameParsing("http://hentai-chan.me/manga/23083-ponpharse-tokubetsu-hen-chast-1.html",
        "Ponpharse - Tokubetsu Hen");

      // Манга требующая регистрации для просмотра
      TestNameParsing("http://hentai-chan.me/manga/14212-love-and-devil-glava-25.html",
        "Love and Devil");
    }

    private void TestNameParsing(string uri, string name)
    {
      ConfigStorage.Instance.AppConfig.Language = Languages.English;
      var manga = GetManga(uri);
      Assert.AreEqual(name, manga.Name);
    }

    private Hentaichan.Hentaichan GetManga(string url)
    {
      var manga = Mangas.CreateFromWeb(new Uri(url)) as Hentaichan.Hentaichan;
      return manga;
    }

    private void Login()
    {
      using (Repository.GetEntityContext())
      {
        var userId = "235332";
        var setting = ConfigStorage.GetPlugin<Hentaichan.Hentaichan>().GetSettings();
        var login = setting.Login as Hentaichan.HentaichanLogin;
        if (login.UserId != userId)
        {
          login.UserId = userId;
          login.PasswordHash = "0578caacc02411f8c9a1a0af31b3befa";
          login.IsLogined = true;
          setting.Save();
        }
      }
    }

    private void Unlogin()
    {
      using (Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Hentaichan.Hentaichan>().GetSettings();
        var login = setting.Login as Hentaichan.HentaichanLogin;
        login.UserId = "";
        login.PasswordHash = "";
        login.IsLogined = false;
        setting.Save();
      }
    }
  }
}