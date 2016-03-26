using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaReader.Account;
using MangaReader.Services;

namespace MangaReader.Manga.Acomic
{
  public class AcomicsLogin : Login
  {
    public new static Guid Type { get { return Guid.Parse("F526CD85-7846-4F32-85A7-C57E3983DFB1"); } }

    public new static Guid Manga { get { return Acomics.Type; } }

    public virtual string PasswordHash { get; set; }

    public override async Task<bool> DoLogin()
    {
      await base.DoLogin();
      if (IsLogined || !this.CanLogin)
        return IsLogined;

      var loginData = new NameValueCollection
            {
                {"submit", "login"},
                {"username", this.Name},
                {"password", this.Password},
                {"check", "1"}
            };

      using (TimedLock.Lock(this.ClientLock))
      {
        try
        {
          await Client.UploadValuesTaskAsync("http://acomics.ru/action/authLogin", "POST", loginData);
          this.PasswordHash = Client.Cookie.GetCookies(this.MainUri)
              .Cast<Cookie>()
              .Single(c => c.Name == "hash")
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

    protected override async Task<List<Mangas>> DownloadBookmarks()
    {
      var bookmarks = await base.DownloadBookmarks();
      var document = new HtmlDocument();

      await this.DoLogin();

      if (!IsLogined)
        return bookmarks;

      using (TimedLock.Lock(ClientLock))
      {
        var page = await Page.GetPageAsync(BookmarksUri, Client);
        document.LoadHtml(page.Content);
      }

      var nodes = document.DocumentNode.SelectNodes("//table[@class=\"decor\"]//a");

      if (nodes == null)
      {
        Log.Add(string.Format("Bookmarks from '{0}' not found.", this.MainUri));
        return bookmarks;
      }

      foreach (var node in nodes)
      {
        var name = WebUtility.HtmlDecode(node.ChildNodes.Single().InnerText);
        var url = node.Attributes.Single().Value;
        var manga = new Acomics {Uri = new Uri(this.MainUri, url), Name = name};
        bookmarks.Add(manga);
      }

      return bookmarks;
    }

    public AcomicsLogin()
    {
      this.MainUri = new Uri("http://acomics.ru/");
      this.LogoutUri = new Uri(this.MainUri + "auth/logout");
      this.BookmarksUri = new Uri(this.MainUri + "settings/subscribes");
    }
  }
}