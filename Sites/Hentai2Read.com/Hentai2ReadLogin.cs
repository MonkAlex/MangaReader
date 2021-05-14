using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaReader.Core.Account;
using MangaReader.Core.Manga;
using MangaReader.Core.Properties;
using MangaReader.Core.Services;

namespace Hentai2Read.com
{
  public class Hentai2ReadLogin : Login
  {
    public override Uri MainUri { get; set; }
    public override Uri LogoutUri { get { return new Uri(this.MainUri, "logout"); } }
    public override Uri BookmarksUri { get { return new Uri(this.MainUri, "bookmark"); } }

    internal string LogoutNonce { get; set; }

    public override async Task<bool> DoLogin(Guid mangaType)
    {
      var isLogined = this.IsLogined(mangaType);
      if (isLogined || !this.CanLogin)
        return isLogined;

      var loginData = new Dictionary<string, string>()
            {
              {"action", "login" },
              {"log", this.Name },
              {"pwd", this.Password },
              {"rememberme", "forever" },
              {"wp-submit", "" },
              {"instance", "" },
              {"redirect_to", BookmarksUri.OriginalString },
            };

      try
      {
        var cookieClient = await Hentai2ReadPlugin.Instance.GetCookieClient(false).ConfigureAwait(false);
        var loginResult = await cookieClient.Post(new Uri(this.MainUri + "login"), loginData).ConfigureAwait(false);
        LogoutNonce = Regex.Match(loginResult.Content, "logout\\/\\?_wpnonce=([a-z0-9]+)&", RegexOptions.Compiled).Groups[1].Value;
        isLogined = cookieClient.CookieContainer.GetCookies(this.MainUri)
          .Cast<Cookie>()
          .Any(c => c.Name.StartsWith("wordpress_logged_in"));
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, Strings.Login_Failed);
        isLogined = false;
      }
      this.SetLogined(mangaType, isLogined);
      return isLogined;
    }

    public override async Task<bool> Logout(Guid mangaType)
    {
      // https://hentai2read.com/logout/?_wpnonce=368febb749
      this.SetLogined(mangaType, false);
      var cookieClient = await Hentai2ReadPlugin.Instance.GetCookieClient(false).ConfigureAwait(false);
      await cookieClient.GetPage(new Uri(LogoutUri.OriginalString + $"/?_wpnonce={LogoutNonce}")).ConfigureAwait(false);
      return true;
    }

    protected override async Task<List<IManga>> DownloadBookmarks(Guid mangaType)
    {
      var bookmarks = new List<IManga>();
      var document = new HtmlDocument();

      var isLogined = await this.DoLogin(mangaType).ConfigureAwait(false);

      if (!isLogined)
        return bookmarks;

      var cookieClient = await Hentai2ReadPlugin.Instance.GetCookieClient(false).ConfigureAwait(false);
      var page = await cookieClient.GetPage(BookmarksUri).ConfigureAwait(false);
      document.LoadHtml(page.Content);

      var nodes = document.DocumentNode.SelectNodes("//div[@class=\"col-xs-6 col-sm-4 col-md-3 col-xl-2\"]");

      if (nodes == null)
      {
        Log.AddFormat("Bookmarks from '{0}' not found.", this.MainUri);
        return bookmarks;
      }

      var parser = new Hentai2ReadParser();
      foreach (var node in nodes)
      {
        var manga = await parser.GetMangaFromBookmarks(MainUri, null, node).ConfigureAwait(false);
        bookmarks.Add(manga);
      }

      return bookmarks;
    }

    public Hentai2ReadLogin()
    {
      // Адрес может быть переопределен в базе. Это только дефолтное значение.
      this.MainUri = new Uri("https://hentai2read.com/");
    }
  }
}
