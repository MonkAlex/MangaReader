using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;

namespace Hentaichan.Mangachan
{
  public class MangachanLogin : BaseLogin
  {

    /// <summary>
    /// https://manga-chan.me//user/RandomUserName/favorites
    /// </summary>
    public override Uri BookmarksUri { get { return new Uri(this.MainUri, $"user/{Name}/favorites"); } }

    protected override async Task<List<IManga>> DownloadBookmarks()
    {
      var bookmarks = new List<IManga>();
      var document = new HtmlDocument();

      await this.DoLogin().ConfigureAwait(false);

      if (!IsLogined)
        return bookmarks;

      var page = await Page.GetPageAsync(BookmarksUri, GetClient()).ConfigureAwait(false);
      document.LoadHtml(page.Content);

      var nodes = document.DocumentNode
        .SelectNodes("//div[@style=\"float:left;width:810px;margin-top:3px;\"]//a[not(contains(@style,'font-size:11px;'))]");

      if (nodes == null)
      {
        Log.AddFormat("Bookmarks from '{0}' not found.", this.MainUri);
        return bookmarks;
      }

      await Task.WhenAll(nodes.Select(async u =>
      {
#warning HACK for https in bookmarks
        var manga = await Mangas.Create(new Uri(MainUri, new Uri(u.Attributes[0].Value).PathAndQuery)).ConfigureAwait(false);
        if (manga == null)
          return;
        manga.ServerName = WebUtility.HtmlDecode(u.InnerText);
        bookmarks.Add(manga);
      })).ConfigureAwait(false);

      return bookmarks;
    }

    public MangachanLogin()
    {
      // Адрес может быть переопределен в базе. Это только дефолтное значение.
      this.MainUri = new Uri(@"https://manga-chan.me/");
    }
  }
}
