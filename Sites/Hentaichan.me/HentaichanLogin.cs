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
using MangaReader.Core.Manga;
using MangaReader.Core.Services;

namespace Hentaichan
{
  public class HentaichanLogin : BaseLogin
  {
    protected override async Task<List<IManga>> DownloadBookmarks()
    {
      var bookmarks = new List<IManga>();
      var document = new HtmlDocument();

      await this.DoLogin().ConfigureAwait(false);

      if (!IsLogined)
        return bookmarks;

      var pages = new List<Uri>() { BookmarksUri };

      for (int i = 0; i < pages.Count; i++)
      {
        var page = await Page.GetPageAsync(pages[i], GetClient()).ConfigureAwait(false);
        document.LoadHtml(page.Content);

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

        var parser = new Parser();
        var mangas = nodes
          .Select(n => n.OuterHtml)
          .SelectMany(h => Regex.Matches(h, "href=\"(.*?)\"", RegexOptions.IgnoreCase)
                             .OfType<Group>()
                             .Select(g => g.Captures[0])
                             .OfType<Match>()
                             .Select(m => new Uri(m.Groups[1].Value)));

        await Task.WhenAll(mangas.Select(async m =>
        {
          var manga = await Mangas.Create(m).ConfigureAwait(false);
          if (manga == null)
            return;

          await parser.UpdateNameAndStatus(manga).ConfigureAwait(false);
          if (!string.IsNullOrWhiteSpace(manga.Name))
            bookmarks.Add(manga);
        })).ConfigureAwait(false);
      }

      return bookmarks;
    }

    public HentaichanLogin()
    {
      // Адрес может быть переопределен в базе. Это только дефолтное значение.
      this.MainUri = new Uri(@"http://h-chan.me/");
    }
  }
}
