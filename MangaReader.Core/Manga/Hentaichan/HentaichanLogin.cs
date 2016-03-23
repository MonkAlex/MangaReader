using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaReader.Account;
using MangaReader.Services;

namespace MangaReader.Manga.Hentaichan
{
  public class HentaichanLogin : Login
  {
    public new static Guid Type { get { return Guid.Parse("03CEFF67-1472-438A-A90A-07B44F6FFDC4"); } }

    public new static Guid Manga { get { return Hentaichan.Type; } }

    public virtual string UserId { get; set; }

    public virtual string PasswordHash
    {
      get
      {
        if (string.IsNullOrWhiteSpace(this.hash))
          this.hash = this.GetPasswordHash();
        return this.hash;
      }
      set { this.hash = value; }
    }

    private string hash = string.Empty;

    public virtual string GetPasswordHash()
    {
      if (string.IsNullOrWhiteSpace(this.Password))
        return string.Empty;

      using (var md5 = MD5.Create())
      {
        return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(this.Password))).Replace("-", "").ToLower();
      }
    }

    public override async Task<bool> DoLogin()
    {
      await base.DoLogin();
      if (IsLogined || !this.CanLogin)
        return IsLogined;

      var loginData = new NameValueCollection
            {
                {"login", "submit"},
                {"login_name", this.Name},
                {"login_password", this.Password},
                {"image", "%D0%92%D1%85%D0%BE%D0%B4"}
            };

      using (TimedLock.Lock(this.ClientLock))
      {
        try
        {
          await Client.UploadValuesTaskAsync(MainUri, "POST", loginData);
          this.UserId = Client.Cookie.GetCookies(new Uri(@"http:\\hentaichan.ru"))
              .Cast<Cookie>()
              .Single(c => c.Name == "dle_user_id")
              .Value;
          this.IsLogined = true;
        }
        catch (Exception ex)
        {
          Log.Exception(ex);
          this.IsLogined = false;
        }
      }
      return IsLogined;
    }

    public override async Task<List<Mangas>> GetBookmarks()
    {
      var bookmarks = await base.GetBookmarks();
      var document = new HtmlDocument();

      await this.DoLogin();

      if (!IsLogined)
        return bookmarks;

      var pages = new List<Uri>() {BookmarksUri};

      for (int i = 0; i < pages.Count; i++)
      {
        using (TimedLock.Lock(ClientLock))
        {
          var page = await Page.GetPageAsync(pages[i], Client);
          document.LoadHtml(page.Content);
        }

        if (i == 0)
        {
          var pageNodes = document.DocumentNode.SelectNodes("//div[@class=\"navigation\"]//a");
          if (pageNodes != null)
          {
            foreach (var node in pageNodes)
            {
              pages.Add(new Uri(node.Attributes[0].Value));
            }
            pages = pages.Distinct().ToList();
          }
        }

        var nodes = document.DocumentNode.SelectNodes("//div[@class=\"manga_row1\"]");

        if (nodes == null)
          return bookmarks;

        foreach (var html in nodes.Select(n => n.OuterHtml))
        {
          var loadedBookmarks = Regex
              .Matches(html, "href=\"(.*?)\"", RegexOptions.IgnoreCase)
              .OfType<Group>()
              .Select(g => g.Captures[0])
              .OfType<Match>()
              .Select(m => new Uri(m.Groups[1].Value.Replace("/manga/", "/related/")))
              .Select(s => new Hentaichan { Uri = s, Name = Getter.GetMangaName(s) })
              .Where(m => !string.IsNullOrWhiteSpace(m.Name))
              .ToList();
          bookmarks.AddRange(loadedBookmarks);
        }
      }

      return bookmarks;
    }

    public HentaichanLogin()
    {
      this.MainUri = new Uri(@"http://hentaichan.ru/index.php");
      this.LogoutUri = new Uri(this.MainUri + "?action=logout");
      this.BookmarksUri = new Uri(@"http://hentaichan.ru/favorites/");
    }
  }
}