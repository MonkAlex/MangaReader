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

    protected override CookieClient GetClient()
    {
      return new Hentai2ReadClient() { BaseAddress = MainUri.ToString(), Cookie = this.ClientCookie };
    }

    public override async Task<bool> DoLogin()
    {
      if (IsLogined || !this.CanLogin)
        return IsLogined;

      var loginData = new NameValueCollection
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
        var loginResult = await GetClient().UploadValuesTaskAsync(new Uri(this.MainUri + "login"), "POST", loginData).ConfigureAwait(false);
        LogoutNonce = Regex.Match(System.Text.Encoding.UTF8.GetString(loginResult), "logout\\/\\?_wpnonce=([a-z0-9]+)&", RegexOptions.Compiled).Groups[1].Value;
        var hasLoginCookie = ClientCookie.GetCookies(this.MainUri)
          .Cast<Cookie>()
          .Any(c => c.Name.StartsWith("wordpress_logged_in"));
        this.IsLogined = hasLoginCookie;
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, Strings.Login_Failed);
        this.IsLogined = false;
      }
      return IsLogined;
    }

    public override async Task<bool> Logout()
    {
      // https://hentai2read.com/logout/?_wpnonce=368febb749
      IsLogined = false;
      await Page.GetPageAsync(new Uri(LogoutUri.OriginalString + $"/?_wpnonce={LogoutNonce}"), GetClient()).ConfigureAwait(false);
      return true;
    }

    protected override async Task<List<IManga>> DownloadBookmarks()
    {
      var bookmarks = new List<IManga>();
      var document = new HtmlDocument();

      await this.DoLogin().ConfigureAwait(false);

      if (!IsLogined)
        return bookmarks;

      var page = await Page.GetPageAsync(BookmarksUri, GetClient()).ConfigureAwait(false);
      document.LoadHtml(page.Content);

      var nodes = document.DocumentNode.SelectNodes("//div[@class=\"col-xs-6 col-sm-4 col-md-3 col-lg-2b col-xl-2\"]");

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
