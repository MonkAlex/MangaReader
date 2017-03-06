using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaReader.Core;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Mangachan
{
  public class Parser : BaseSiteParser
  {
    public override void UpdateNameAndStatus(IManga manga)
    {
      var name = string.Empty;
      try
      {
        var document = new HtmlDocument();
        var page = Page.GetPage(manga.Uri);
        document.LoadHtml(page.Content);
        var nameNode = document.DocumentNode.SelectSingleNode("//head/title");
        name = nameNode.InnerText.Split(new[] {"&raquo;"}, StringSplitOptions.RemoveEmptyEntries)[0];
        name = name.Trim();
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
      name = WebUtility.HtmlDecode(name);

      this.UpdateName(manga, name);
    }

    public override void UpdateContent(IManga manga)
    {
      //throw new NotImplementedException();
    }

    public override UriParseResult ParseUri(Uri uri)
    {
      // Manga : http://mangachan.me/manga/31910-jigoku-koi-sutefu.html
      // Volume : -
      // Chapter : http://mangachan.me/online/249080-jigoku-koi-sutefu_v1_ch6.5.html
      // Page : -

      var hosts = ConfigStorage.Plugins
        .Where(p => p.GetParser().GetType() == typeof(Parser))
        .SelectMany(p => p.GetSettings().MangaSettingUris);

      foreach (var host in hosts)
      {
        var trimmedHost = host.OriginalString.TrimEnd('/');
        if (!uri.OriginalString.StartsWith(trimmedHost))
          continue;

        var relativeUri = uri.OriginalString.Remove(0, trimmedHost.Length);
        var manga = "/manga/";
        if (relativeUri.Contains(manga))
          return new UriParseResult(true, UriParseKind.Manga, uri);
        var online = "/online/";
        if (relativeUri.Contains(online))
          return new UriParseResult(true, UriParseKind.Chapter, uri);
      }

      return new UriParseResult(false, UriParseKind.Manga, null);
    }
  }
}
