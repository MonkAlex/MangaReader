using System;
using System.Linq;
using Hentaichan;
using MangaReader.Core.Manga;
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
      var manga = GetManga("http://henchan.me/related/14212-love-and-devil-glava-25.html");
      Assert.AreEqual(25, manga.Chapters.Count);
      Assert.IsTrue(manga.HasChapters);
    }

    private Hentaichan.Hentaichan GetManga(string url)
    {
      CreateLogin();
      var manga = Mangas.CreateFromWeb(new Uri(url)) as Hentaichan.Hentaichan;
      return manga;
    }

    private void CreateLogin()
    {
      var setting = ConfigStorage.GetPlugin<Hentaichan.Hentaichan>().GetSettings();
      var login = setting.Login as Hentaichan.HentaichanLogin;
      login.UserId = "235332";
      login.PasswordHash = "0578caacc02411f8c9a1a0af31b3befa";
      login.IsLogined = true;
      setting.Save();
    }
  }
}