using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MangaReader.Account;
using MangaReader.Services;

namespace MangaReader.Manga.Grouple
{
  public class GroupleLogin : Login
  {
    public new static Guid Type { get { return Guid.Parse("0BBE71B1-16E0-44F4-B7C6-3450E44E9A15"); } }

    public override void DoLogin()
    {
      base.DoLogin();
      if (IsLogined || !this.CanLogin)
        return;

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
          var result = Client.UploadValues("internal/auth/j_spring_security_check", "POST", loginData);
          IsLogined = Encoding.UTF8.GetString(result).Contains("internal/auth/logout");
        }
        catch (Exception ex)
        {
          Log.Exception(ex);
        }
      }
    }

    public override List<Mangas> GetBookmarks()
    {
      var bookmarks = base.GetBookmarks();
      var document = new HtmlDocument();

      this.DoLogin();

      if (!IsLogined)
        return bookmarks;

      using (TimedLock.Lock(ClientLock))
      {
        document.LoadHtml(Page.GetPage(BookmarksUri, Client));
      }

      var firstOrDefault = document.DocumentNode
          .SelectNodes("//div[@class=\"bookmarks-lists\"]");

      if (firstOrDefault == null || firstOrDefault.FirstOrDefault() == null)
        return bookmarks;

      var loadedBookmarks = Regex
          .Matches(firstOrDefault.FirstOrDefault().OuterHtml, @"href='(.*?)'", RegexOptions.IgnoreCase)
          .OfType<Group>()
          .Select(g => g.Captures[0])
          .OfType<Match>()
          .Select(m => new Uri(m.Groups[1].Value))
          .Select(s => new Readmanga() { Uri = s, Name = Getter.GetMangaName(Page.GetPage(s)).ToString() })
          .ToList();
      bookmarks.AddRange(loadedBookmarks);
      return bookmarks;
    }

    public GroupleLogin()
    {
      this.MainUri = new Uri(@"http://grouple.ru/");
      this.LogoutUri = new Uri(this.MainUri + "internal/auth/logout");
      this.BookmarksUri = new Uri(@"http://grouple.ru/private/bookmarks");
    }
  }
}