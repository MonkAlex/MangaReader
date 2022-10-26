using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaReader.Core.Account;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Properties;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using Newtonsoft.Json.Linq;

namespace Grouple
{
  public class GroupleLogin : Login
  {
    public override Uri MainUri { get; set; }
    public override Uri LogoutUri { get { return new Uri(this.MainUri, "login/logout"); } }
    public override Uri BookmarksUri { get { return new Uri(this.MainUri, "private/bookmarks"); } }

    public override async Task<bool> DoLogin(Guid mangaType)
    {
      var isLogined = this.IsLogined(mangaType);
      if (isLogined || !this.CanLogin)
        return isLogined;

      try
      {
        var plugin = ConfigStorage.Plugins.Single(p => p.MangaGuid == mangaType) as IGrouplePlugin;
        var client = await plugin.GetCookieClient(false).ConfigureAwait(false);
        var loginData = new Dictionary<string, string>()
        {
          {"username", this.Name},
          {"password", this.Password},
          {"remember_me", "true"},
          {"_remember_me_yes", ""},
          {"remember_me_yes", "on"},
          {"targetUri", $"/login/continueSso?targetUri=https%3A%2F%2F{this.MainUri.Host}%2F&siteId={plugin.SiteId}"}
        };
        var result = await client.Post(new Uri(this.MainUri, "login/authenticate"), loginData).ConfigureAwait(false);
        isLogined = result.Content.Contains("login/logout");
        this.SetLogined(mangaType, isLogined);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, Strings.Login_Failed);
      }
      return isLogined;
    }

    protected override async Task<List<IManga>> DownloadBookmarks(Guid mangaType)
    {
      var bookmarks = new List<IManga>();

      var isLogined = await this.DoLogin(mangaType).ConfigureAwait(false);

      if (!isLogined)
        return bookmarks;

      var plugin = ConfigStorage.Plugins.Single(p => p.MangaGuid == mangaType) as IGrouplePlugin;
      var client = await plugin.GetCookieClient(false).ConfigureAwait(false);
      var gwt = client.GetCookie("gwt");
      // {"bookmarkSort":"NAME","elementFilter":[],"statusFilter":["WATCHING"],"limit":50,"offset":0}
      var page = await client.Post(new Uri("https://grouple.co/api/bookmark/list"), parameters: new Dictionary<string, string>
      {
        { "bookmarkSort", "NAME" },
        { "limit", "50" },
        { "offset", "0" },
      },
      new Dictionary<string, string> { { "Authorization", "Bearer " + gwt } }).ConfigureAwait(false);

      if (!page.HasContent)
      {
        Log.AddFormat("Bookmarks from '{0}' not found.", this.MainUri);
        return bookmarks;
      }

      using (var context = Repository.GetEntityContext("Loading bookmarks"))
      {
        var loadedBookmarks = new List<Uri>();
        var jsonParsed = JObject.Parse(page.Content)["list"].ToList();
        var settings = plugin.GetSettings();
        for (var i = 0; i < jsonParsed.Count; i++)
        {
          var child = jsonParsed[i];
          var element = child["element"]["elementId"];
          var link = element["linkName"].Value<string>();
          var site = element["siteId"].Value<int>();
          var status = child["status"].Value<string>();
          if (plugin.SiteId == site && status.ToUpperInvariant() != "DROPPED")
          {
            loadedBookmarks.Add(new Uri(settings.MainUri, link));
          }
        }

        var parser = plugin.GetParser();
        await Task.WhenAll(loadedBookmarks.Select(async b =>
        {
          var manga = await Mangas.Create(b).ConfigureAwait(false);
          if (manga.Setting.Manga != mangaType)
            return;
          await parser.UpdateNameAndStatus(manga).ConfigureAwait(false);
          bookmarks.Add(manga);
        })).ConfigureAwait(false);
      }
      return bookmarks;
    }

    public GroupleLogin()
    {
      // Адрес может быть переопределен в базе. Это только дефолтное значение.
      this.MainUri = new Uri(@"https://grouple.co/");
    }
  }
}
