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
using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace MangaReader.Core.Manga.Hentaichan
{
  public class HentaichanLogin : Login
  {
    public new static Guid Type { get { return Guid.Parse("03CEFF67-1472-438A-A90A-07B44F6FFDC4"); } }

    public new static Guid Manga { get { return Hentaichan.Type; } }

    public virtual string UserId { get; set; }

    public override string Name
    {
      get { return name; }
      set
      {
        if (name == value)
          return;

        name = value;
        this.UserId = string.Empty;
      }
    }

    public override string Password
    {
      get { return password; }
      set
      {
        if (password == value)
          return;

        password = value;
        this.PasswordHash = string.Empty;
      }
    }

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
    private string password;
    private string name;

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
          this.UserId = Client.Cookie.GetCookies(new Uri(@"http:\\hentaichan.me"))
              .Cast<Cookie>()
              .Single(c => c.Name == "dle_user_id")
              .Value;
          this.IsLogined = true;
        }
        catch (System.Exception ex)
        {
          Log.Exception(ex);
          this.IsLogined = false;
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

      var pages = new List<Uri>() { BookmarksUri };

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
        {
          Log.AddFormat("Bookmarks from '{0}' not found.", this.MainUri);
          return bookmarks;
        }

        var mangas = nodes
          .Select(n => n.OuterHtml)
          .SelectMany(h => Regex.Matches(h, "href=\"(.*?)\"", RegexOptions.IgnoreCase)
                             .OfType<Group>()
                             .Select(g => g.Captures[0])
                             .OfType<Match>()
                             .Select(m => new Uri(m.Groups[1].Value.Replace("/manga/", "/related/"))))
          .Select(u => new Hentaichan { Uri = u, Name = Getter.GetMangaName(u) })
          .Where(m => !string.IsNullOrWhiteSpace(m.Name))
          .ToList();

        bookmarks.AddRange(mangas);
      }

      return bookmarks;
    }

    public HentaichanLogin()
    {
      this.MainUri = new Uri(@"http://hentaichan.me/index.php");
      this.LogoutUri = new Uri(this.MainUri + "?action=logout");
      this.BookmarksUri = new Uri(@"http://hentaichan.me/favorites/");
    }
  }
}