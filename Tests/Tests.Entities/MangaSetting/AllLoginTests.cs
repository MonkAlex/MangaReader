using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acomics;
using Grouple;
using Hentai2Read.com;
using Hentaichan;
using Hentaichan.Mangachan;
using MangaReader.Core.Account;
using NUnit.Framework;

namespace Tests.Entities.MangaSetting
{
  [TestFixture]
  public class AllLoginTests : TestClass
  {
    public static IEnumerable<(ILogin, Guid)> GetLogins()
    {
      // See issue 174
      //if (!AppveyorHelper.IsRunning())
      //  yield return (new GroupleLogin() { Name = "alex+grouple@antistarforce.com", Password = "JUadiSHrosiv" }, MintmangaPlugin.Manga);
      //if (!AppveyorHelper.IsRunning())
      //  yield return (new GroupleLogin() { Name = "alex+grouple@antistarforce.com", Password = "JUadiSHrosiv" }, ReadmangaPlugin.Manga);
      yield return (new AcomicsLogin() { Name = "v924147", Password = "ocUsigairtYL" }, AcomicsPlugin.Manga);
      yield return (new HentaichanLogin() { Name = "v924147", Password = "OViLKHoTCHBL" }, HentaichanPlugin.Manga);
      yield return (new MangachanLogin() { Name = "v924147", Password = "gOWElaTERSIt" }, MangachanPlugin.Manga);
      yield return (new Hentai2ReadLogin() { Name = "v924147", Password = "ToRTUbiCONDe" }, Hentai2ReadPlugin.Manga);
    }

    [Test, TestCaseSource(nameof(GetLogins))]
    public async Task LoginUnlogin((ILogin, Guid) loginTuple)
    {
      var (login, type) = loginTuple;
      Assert.IsTrue(login.CanLogin);
      Assert.IsFalse(login.IsLogined(type));
      var loginResult = await login.DoLogin(type).ConfigureAwait(false);
      Assert.IsTrue(loginResult);
      Assert.IsTrue(login.CanLogin);
      Assert.IsTrue(login.IsLogined(type));
      var logoutResult = await login.Logout(type).ConfigureAwait(false);
      Assert.IsTrue(logoutResult);
      Assert.IsTrue(login.CanLogin);
      Assert.IsFalse(login.IsLogined(type));
    }

    [Test, TestCaseSource(nameof(GetLogins))]
    public async Task GetBookmarks((ILogin, Guid) loginTuple)
    {
      var (login, type) = loginTuple;
      await login.DoLogin(type).ConfigureAwait(false);
      Assert.IsTrue(login.IsLogined(type));

      var bookmarks = await login.GetBookmarks(type).ConfigureAwait(false);
      Assert.AreEqual(2, bookmarks.Count);
      foreach (var manga in bookmarks)
      {
        Assert.IsNotNull(manga.Name);
        Assert.IsNotNull(manga.Uri);
      }
    }

    [Test, MintManga, ReadManga, Grouple, Issue(174)]
    public async Task SingleLoginForManySites()
    {
      var login = new GroupleLogin() { Name = "alex+grouple@antistarforce.com", Password = "JUadiSHrosiv" };

      var bookmarks = await login.GetBookmarks(MintmangaPlugin.Manga).ConfigureAwait(false);
      Assert.AreEqual(2, bookmarks.Count);

      bookmarks = await login.GetBookmarks(ReadmangaPlugin.Manga).ConfigureAwait(false);
      Assert.AreEqual(2, bookmarks.Count);
    }
  }
}
