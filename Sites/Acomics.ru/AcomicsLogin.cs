using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaReader.Core.Account;
using MangaReader.Core.Manga;
using MangaReader.Core.Properties;
using MangaReader.Core.Services;

namespace Acomics
{
  public class AcomicsLogin : Login
  {
    public virtual string PasswordHash { get; set; }

    public override Uri MainUri { get; set; }
    public override Uri LogoutUri { get { return new Uri(this.MainUri, "auth/logout"); } }
    public override Uri BookmarksUri { get { return new Uri(this.MainUri, "settings/subscribes"); } }

    public override async Task<bool> DoLogin()
    {
      if (IsLogined || !this.CanLogin)
        return IsLogined;

      var loginData = new NameValueCollection
            {
                {"submit", "login"},
                {"username", this.Name},
                {"password", this.Password},
                {"check", "1"}
            };

      try
      {
        var cookieClient = AcomicsPlugin.Instance.GetCookieClient();
        await cookieClient.UploadValuesTaskAsync(new Uri(this.MainUri + "action/authLogin"), "POST", loginData).ConfigureAwait(false);
        this.PasswordHash = cookieClient.Cookie.GetCookies(this.MainUri)
            .Cast<Cookie>()
            .Single(c => c.Name == "hash")
            .Value;
        this.IsLogined = true;
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, Strings.Login_Failed);
        this.IsLogined = false;
      }
      return IsLogined;
    }

    protected override async Task<List<IManga>> DownloadBookmarks()
    {
      var bookmarks = new List<IManga>();
      var document = new HtmlDocument();

      await this.DoLogin().ConfigureAwait(false);

      if (!IsLogined)
        return bookmarks;

      var cookieClient = AcomicsPlugin.Instance.GetCookieClient();
      var page = await Page.GetPageAsync(BookmarksUri, cookieClient).ConfigureAwait(false);
      document.LoadHtml(page.Content);

      var nodes = document.DocumentNode.SelectNodes("//table[@class=\"decor\"]//a");

      if (nodes == null)
      {
        Log.AddFormat("Bookmarks from '{0}' not found.", this.MainUri);
        return bookmarks;
      }

      foreach (var node in nodes)
      {
        var name = WebUtility.HtmlDecode(node.ChildNodes.Single().InnerText);
        var url = node.Attributes.Single().Value;
        var manga = await Mangas.Create(new Uri(this.MainUri, url)).ConfigureAwait(false);
        manga.Name = name;
        bookmarks.Add(manga);
      }

      return bookmarks;
    }

    public AcomicsLogin()
    {
      // Адрес может быть переопределен в базе. Это только дефолтное значение.
      this.MainUri = new Uri("https://acomics.ru/");
    }
  }
}
