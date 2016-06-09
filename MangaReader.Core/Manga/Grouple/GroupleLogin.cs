using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace MangaReader.Core.Manga.Grouple
{
  public class GroupleLogin : Login
  {
    public new static Guid Type { get { return Guid.Parse("0BBE71B1-16E0-44F4-B7C6-3450E44E9A15"); } }

    public new static Guid[] Manga { get { return new Guid[2] {Readmanga.Type, Mintmanga.Type}; } }

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
      using (TimedLock.Lock(ClientLock))
      {
        try
        {
          var result = await Client.UploadValuesTaskAsync("internal/auth/j_spring_security_check", "POST", loginData);
          IsLogined = Encoding.UTF8.GetString(result).Contains("internal/auth/logout");
        }
        catch (System.Exception ex)
        {
          Log.Exception(ex);
        }
      }
      return IsLogined;
    }

    protected override async Task<List<Mangas>> DownloadBookmarks()
    {
      var bookmarks = new List<Mangas>();
      var document = new HtmlDocument();

      await this.DoLogin();

      if (!IsLogined)
        return bookmarks;

      using (TimedLock.Lock(ClientLock))
      {
        var page = await Page.GetPageAsync(BookmarksUri, Client);
        document.LoadHtml(page.Content);
      }

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
            var page = await Page.GetPageAsync(s);
            var manga = Mangas.Create(s);
            manga.Name = Getter.GetMangaName(page.Content).ToString();
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
#warning 55
      this.MainUri = new Uri(@"http://grouple.ru/");
      this.LogoutUri = new Uri(this.MainUri, "internal/auth/logout");
      this.BookmarksUri = new Uri(this.MainUri, "private/bookmarks");
    }
  }
}