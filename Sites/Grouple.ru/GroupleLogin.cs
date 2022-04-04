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
      var document = new HtmlDocument();

      var isLogined = await this.DoLogin(mangaType).ConfigureAwait(false);

      if (!isLogined)
        return bookmarks;

      var plugin = ConfigStorage.Plugins.Single(p => p.MangaGuid == mangaType);
      var client = await plugin.GetCookieClient(false).ConfigureAwait(false);
      var page = await client.GetPage(BookmarksUri).ConfigureAwait(false);
      document.LoadHtml(page.Content);

      var firstOrDefault = document.DocumentNode
          .SelectNodes("//div[@class=\"bookmarks-lists\"]");

      var bookMarksNode = firstOrDefault?.FirstOrDefault();
      if (bookMarksNode == null)
      {
        Log.AddFormat("Bookmarks from '{0}' not found.", this.MainUri);
        return bookmarks;
      }

      var parser = plugin.GetParser();
      using (var context = Repository.GetEntityContext("Loading bookmarks"))
      {
        var loadedBookmarks = Regex
          .Matches(bookMarksNode.OuterHtml, @"href='(.*?)'", RegexOptions.IgnoreCase)
          .OfType<Group>()
          .Select(g => g.Captures[0])
          .OfType<Match>()
          .Select(m => new Uri(m.Groups[1].Value));

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
