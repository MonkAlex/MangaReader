using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Mangachan
{
  public class Parser : BaseSiteParser
  {
    private static CookieClient GetClient()
    {
      var setting = ConfigStorage.GetPlugin<Mangachan>().GetSettings();
      var client = new CookieClient();
      if (setting != null)
      {
        var login = setting.Login as BaseLogin;
        if (!login.CanLogin || string.IsNullOrWhiteSpace(login.UserId))
        {
          login.DoLogin().Wait();
        }
        if (!string.IsNullOrWhiteSpace(login.UserId))
        {
          var host = Generic.GetLoginMainUri<Mangachan>().Host;
          client.Cookie.Add(new Cookie("dle_user_id", login.UserId, "/", host));
          client.Cookie.Add(new Cookie("dle_password", login.PasswordHash, "/", host));
        }
      }
      return client;
    }

    public override void UpdateNameAndStatus(IManga manga)
    {
      var localizedName = new MangaName();
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(Page.GetPage(manga.Uri).Content);
        var html = document.DocumentNode.InnerHtml;
        var enName = Regex.Match(html, @"title>(.*?) &raquo", RegexOptions.IgnoreCase);
        if (enName.Success)
        {
          localizedName.English = WebUtility.HtmlDecode(enName.Groups[1].Value);
          var ruName = Regex.Match(html, $"{localizedName.English} \\((.*?)\\)", RegexOptions.IgnoreCase);
          if (ruName.Success)
            localizedName.Russian = WebUtility.HtmlDecode(ruName.Groups[1].Value);
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }

      this.UpdateName(manga, localizedName.ToString());
    }

    public override void UpdateContent(IManga manga)
    {
      var chapters = new List<Chapter>();
      try
      {
        var document = new HtmlDocument();
        var content = Page.GetPage(manga.Uri, GetClient()).Content;
        document.LoadHtml(content);

        var chapterNodes = document.DocumentNode.SelectNodes("//table[@class=\"table_cha\"]//a");
        if (chapterNodes != null)
        {
          foreach (var node in chapterNodes)
          {
            var link = node.Attributes.Single(a => a.Name == "href").Value;
            chapters.Add(new Chapter(new Uri(manga.Uri, link), WebUtility.HtmlDecode(node.InnerText)));
          }
          chapters.Reverse();
        }
      }
      catch (NullReferenceException ex)
      {
        Log.Exception(ex, $"Возможно, требуется регистрация для доступа к {manga.Uri}");
      }

      foreach (var volume in chapters.GroupBy(c => c.Volume).ToList())
      {
        var vol = new Volume(volume.Key);
        vol.Chapters.AddRange(volume);
        chapters.RemoveAll(c => volume.Contains(c));
        manga.Volumes.Add(vol);
      }

      manga.Chapters.AddRange(chapters);
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

    public static void UpdatePages(MangaReader.Core.Manga.Chapter chapter)
    {
      chapter.Pages.Clear();
      var pages = new List<MangaPage>();
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(Page.GetPage(chapter.Uri, GetClient()).Content);

        var i = 0;
        var imgs = Regex.Match(document.DocumentNode.OuterHtml, @"""(fullimg.*)", RegexOptions.IgnoreCase).Groups[1].Value.Remove(0, 9);
        foreach (Match match in Regex.Matches(imgs, @"""(.*?)"","))
        {
          pages.Add(new MangaPage(chapter.Uri, new Uri(match.Groups[1].Value), i++));
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }

      chapter.Pages.AddRange(pages);
    }
  }
}
