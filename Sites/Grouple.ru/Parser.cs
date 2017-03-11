using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using Newtonsoft.Json.Linq;

namespace Grouple
{
  public class Parser : BaseSiteParser
  {
    /// <summary>
    /// Ключ с куками для редиректа.
    /// </summary>
    internal static string CookieKey = "red-form";

    /// <summary>
    /// Получить ссылку с редиректа.
    /// </summary>
    /// <param name="page">Содержимое страницы по ссылке.</param>
    /// <returns>Новая ссылка.</returns>
    public static Uri GetRedirectUri(Page page)
    {
      var fullUri = page.ResponseUri.OriginalString;

      CookieClient client = null;
      try
      {
        client = new CookieClient();
        var cookie = new Cookie
        {
          Name = CookieKey,
          Value = "true",
          Expires = DateTime.Today.AddYears(1),
          Domain = "." + page.ResponseUri.Host
        };
        client.Cookie.Add(cookie);

        // Пытаемся найти переход с обычной манги на взрослую. Или хоть какой то переход.
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        var node = document.DocumentNode.SelectSingleNode("//form[@id='red-form']");
        if (node != null)
        {
          var actionUri = node.Attributes.FirstOrDefault(a => a.Name == "action").Value;
          fullUri = page.ResponseUri.GetLeftPart(UriPartial.Authority) + actionUri;
        }
        client.UploadValues(fullUri, "POST", new NameValueCollection { { "_agree", "on" }, { "agree", "on" } });
      }
      catch (WebException ex)
      {
        if (!Page.DelayOnException(ex))
          throw;

        return GetRedirectUri(page);
      }

      return client?.ResponseUri;
    }

    /// <summary>
    /// Получить ссылки на все изображения в главе.
    /// </summary>
    /// <param name="chapter">Глава.</param>
    /// <returns>Список ссылок на изображения главы.</returns>
    public static void UpdatePages(Chapter chapter)
    {
      chapter.Pages.Clear();
      var document = new HtmlDocument();
      document.LoadHtml(Page.GetPage(chapter.Uri).Content);
      var node = document.DocumentNode.SelectNodes("//div[@class=\"pageBlock container reader-bottom\"]").FirstOrDefault();
      if (node == null)
        return;

      var initBlock = Regex.Match(node.OuterHtml, @"rm_h\.init\([ ]*(\[\[.*?\]\])", RegexOptions.IgnoreCase);
      var jsonParsed = JToken.Parse(initBlock.Groups[1].Value).Children().ToList();
      for (var i = 0; i < jsonParsed.Count; i++)
      {
        var child = jsonParsed[i];
        var uriString = child[1].ToString() + child[0] + child[2];

        // Фикс страницы с цензурой.
        Uri imageLink;
        if (!Uri.TryCreate(uriString, UriKind.Absolute, out imageLink))
          imageLink = new Uri(@"http://" + chapter.Uri.Host + uriString);

        chapter.Pages.Add(new MangaPage(chapter.Uri, imageLink, i));
      }
    }

    public override void UpdateNameAndStatus(IManga manga)
    {
      var page = Page.GetPage(manga.Uri);
      var localizedName = new MangaName();
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        var japNode = document.DocumentNode.SelectSingleNode("//span[@class='jp-name']");
        if (japNode != null)
          localizedName.Japanese = WebUtility.HtmlDecode(japNode.InnerText);
        var engNode = document.DocumentNode.SelectSingleNode("//span[@class='eng-name']");
        if (engNode != null)
          localizedName.English = WebUtility.HtmlDecode(engNode.InnerText);
        var rusNode = document.DocumentNode.SelectSingleNode("//span[@class='name']");
        if (rusNode != null)
          localizedName.Russian = WebUtility.HtmlDecode(rusNode.InnerText);
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }

      this.UpdateName(manga, localizedName.ToString());

      var status = string.Empty;
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        var nodes = document.DocumentNode.SelectNodes("//div[@class=\"subject-meta col-sm-7\"]//p");
        if (nodes != null)
          status = nodes.Aggregate(status, (current, node) =>
              current + Regex.Replace(node.InnerText.Trim(), @"\s+", " ").Replace("\n", "") + Environment.NewLine);
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
      manga.Status = status;
    }

    public override void UpdateContent(IManga manga)
    {
      var dic = new Dictionary<Uri, string>();
      var links = new List<Uri> { };
      var description = new List<string> { };
      var page = Page.GetPage(manga.Uri);
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        var linkNodes = document.DocumentNode
          .SelectNodes("//div[@class=\"expandable chapters-link\"]//a[@href]")
          .Reverse()
          .ToList();
        links = linkNodes
            .ConvertAll(r => r.Attributes.ToList().ConvertAll(i => i.Value))
            .SelectMany(j => j)
            .Where(k => k != string.Empty)
            .Select(s => @"http://" + page.ResponseUri.Host + s + "?mature=1")
            .Select(s => new Uri(s))
            .ToList();
        description = linkNodes
            .ConvertAll(r => r.InnerText.Replace("\r\n", string.Empty).Trim())
            .ToList();
      }
      catch (NullReferenceException ex) { Log.Exception(ex, "Ошибка получения списка глав.", page.ResponseUri.ToString()); }
      catch (ArgumentNullException ex) { Log.Exception(ex, "Главы не найдены.", page.ResponseUri.ToString()); }

      for (var i = 0; i < links.Count; i++)
      {
        dic.Add(links[i], description[i]);
      }

      var rmVolumes = dic
        .Select(cs => new Chapter(cs.Key, cs.Value))
        .GroupBy(c => c.Volume)
        .Select(g =>
        {
          var v = new Volume(g.Key);
          v.Chapters.AddRange(g);
          return v;
        });

      manga.Volumes.AddRange(rmVolumes);
    }

    public override UriParseResult ParseUri(Uri uri)
    {
      // Manga : http://readmanga.me/heroes_of_the_western_world__emerald_
      // Volume : -
      // Chapter : http://readmanga.me/heroes_of_the_western_world__emerald_/vol0/0
      // Page : -

      var hosts = ConfigStorage.Plugins
        .Where(p => p.GetParser().GetType() == typeof(Parser))
        .SelectMany(p => p.GetSettings().MangaSettingUris);

      foreach (var host in hosts)
      {
        var trimmedHost = host.OriginalString.TrimEnd('/');
        if (!uri.OriginalString.StartsWith(trimmedHost))
          continue;

        if (uri.Segments.Length > 1)
        {
          var mangaUri = new Uri(host, uri.Segments[1].TrimEnd('/'));
          if (uri.Segments.Length == 4)
            return new UriParseResult(true, UriParseKind.Chapter, mangaUri);
          if (uri.Segments.Length == 2)
            return new UriParseResult(true, UriParseKind.Manga, mangaUri);
        }
      }

      return new UriParseResult(false, UriParseKind.Manga, null);
    }
  }
}
