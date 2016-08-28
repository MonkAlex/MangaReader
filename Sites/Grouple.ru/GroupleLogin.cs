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
using MangaReader.Core.Services;

namespace Grouple
{
  public class GroupleLogin : Login
  {
    public override Uri MainUri { get; set; }
    public override Uri LogoutUri { get { return new Uri(this.MainUri, "internal/auth/logout"); } }
    public override Uri BookmarksUri { get { return new Uri(this.MainUri, "private/bookmarks"); } }

    public override async Task<bool> DoLogin()
    {
      if (IsLogined || !this.CanLogin)
        return IsLogined;

      var loginData = new NameValueCollection
            {
                {"j_username", this.Name},
                {"j_password", this.Password},
                {"remember_me", "checked"}
            };
      try
      {
        var result = await GetClient().UploadValuesTaskAsync("internal/auth/j_spring_security_check", "POST", loginData);
        IsLogined = Encoding.UTF8.GetString(result).Contains("internal/auth/logout");
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex);
      }
      return IsLogined;
    }

    protected override async Task<List<IManga>> DownloadBookmarks()
    {
      var bookmarks = new List<IManga>();
      var document = new HtmlDocument();

      await this.DoLogin();

      if (!IsLogined)
        return bookmarks;

      var page = await Page.GetPageAsync(BookmarksUri, GetClient());
      document.LoadHtml(page.Content);

      var firstOrDefault = document.DocumentNode
          .SelectNodes("//div[@class=\"bookmarks-lists\"]");

      if (firstOrDefault == null || firstOrDefault.FirstOrDefault() == null)
      {
        Log.AddFormat("Bookmarks from '{0}' not found.", this.MainUri);
        return bookmarks;
      }


      var loadedBookmarks = Regex
          .Matches(firstOrDefault.FirstOrDefault().OuterHtml, @"href='(.*?)'", RegexOptions.IgnoreCase)
          .OfType<Group>()
          .Select(g => g.Captures[0])
          .OfType<Match>()
          .Select(m => new Uri(m.Groups[1].Value))
          .Select(async s =>
          {
            var mangaPage = await Page.GetPageAsync(s);
            var manga = Mangas.Create(s);
            manga.Name = Getter.GetMangaName(mangaPage.Content).ToString();
            return manga;
          })
          .ToList();
      foreach (var bookmark in loadedBookmarks)
      {
        bookmarks.Add(await bookmark);
      }
      return bookmarks;
    }

    public GroupleLogin()
    {
      // Адрес может быть переопределен в базе. Это только дефолтное значение.
      this.MainUri = new Uri(@"http://grouple.ru/");
    }
  }
}