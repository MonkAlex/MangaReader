using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MangaReader.Account;
using MangaReader.Services;

namespace MangaReader.Manga.Grouple
{
  public static class Getter
  {
    /// <summary>
    /// Ключ с куками для редиректа.
    /// </summary>
    internal static string CookieKey = "red-form";

    /// <summary>
    /// Получить ссылку с редиректа.
    /// </summary>
    /// <param name="uri">Исходная ссылка.</param>
    /// <param name="page">Содержимое страницы по ссылке.</param>
    /// <returns>Новая ссылка.</returns>
    public static Uri GetRedirectUri(Uri uri, string page)
    {
      var client = new CookieClient();
      var cookie = new Cookie
      {
        Name = CookieKey,
        Value = "true",
        Expires = DateTime.Today.AddYears(1),
        Domain = "." + uri.Host
      };
      client.Cookie.Add(cookie);

      var document = new HtmlDocument();
      document.LoadHtml(page);
      var node = document.DocumentNode.SelectSingleNode("//form[@id='red-form']");
      var actionUri = node.Attributes.FirstOrDefault(a => a.Name == "action").Value;
      var fullUri = uri.GetLeftPart(UriPartial.Authority) + actionUri;
      client.UploadValues(fullUri, "POST", new NameValueCollection() { { "_agree", "on" }, { "agree", "on" } });

      return client.ResponseUri;
    }

    /// <summary>
    /// Получить название манги.
    /// </summary>
    /// <param name="mangaMainPage">Содержимое страницы манги.</param>
    /// <returns>Мультиязыковый класс с именем манги.</returns>
    public static MangaName GetMangaName(string mangaMainPage)
    {
      var name = new MangaName();
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(mangaMainPage);
        var japNode = document.DocumentNode.SelectSingleNode("//span[@class='jp-name']");
        if (japNode != null)
          name.Japanese = WebUtility.HtmlDecode(japNode.InnerText);
        var engNode = document.DocumentNode.SelectSingleNode("//span[@class='eng-name']");
        if (engNode != null)
          name.English = WebUtility.HtmlDecode(engNode.InnerText);
        var rusNode = document.DocumentNode.SelectSingleNode("//span[@class='name']");
        if (rusNode != null)
          name.Russian = WebUtility.HtmlDecode(rusNode.InnerText);
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
      return name;
    }

    public static string GetTranslateStatus(string mangaMainPage)
    {
      var status = string.Empty;
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(mangaMainPage);
        var nodes = document.DocumentNode.SelectNodes("//div[@class=\"subject-meta col-sm-7\"]//p");
        if (nodes != null)
          status = nodes.Aggregate(status, (current, node) =>
              current + Regex.Replace(node.InnerText.Trim(), @"\s+", " ").Replace("\n", "") + Environment.NewLine);
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
      return status;
    }

    /// <summary>
    /// Получить ссылки на главы манги.
    /// </summary>
    /// <param name="mangaMainPage">Содержимое страницы манги.</param>
    /// <param name="uri">Ссылка на мангу.</param>
    /// <returns>Словарь (ссылка, описание).</returns>
    public static Dictionary<Uri, string> GetLinksOfMangaChapters(string mangaMainPage, Uri uri)
    {
      var dic = new Dictionary<Uri, string>();
      var links = new List<Uri> { };
      var description = new List<string> { };
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(mangaMainPage);
        links = document.DocumentNode
            .SelectNodes("//div[@class=\"expandable chapters-link\"]//td[@class=\" \"]//a[@href]")
            .ToList()
            .ConvertAll(r => r.Attributes.ToList().ConvertAll(i => i.Value))
            .SelectMany(j => j)
            .Where(k => k != string.Empty)
            .Select(s => @"http://" + uri.Host + s + "?mature=1")
            .Reverse()
            .Select(s => new Uri(s))
            .ToList();
        description = document.DocumentNode
            .SelectNodes("//div[@class=\"expandable chapters-link\"]//tr//td[@class=\" \"]")
            .Reverse()
            .ToList()
            .ConvertAll(r => r.InnerText.Replace("\r\n", string.Empty).Trim())
            .ToList();
      }
      catch (NullReferenceException ex) { Log.Exception(ex, "Ошибка получения списка глав.", uri.ToString()); }
      catch (ArgumentNullException ex) { Log.Exception(ex, "Главы не найдены.", uri.ToString()); }

      for (var i = 0; i < links.Count; i++)
      {
        dic.Add(links[i], description[i]);
      }

      return dic;
    }

    /// <summary>
    /// Получить ссылки на все изображения в главе.
    /// </summary>
    /// <param name="chapter">Глава.</param>
    /// <returns>Список ссылок на изображения главы.</returns>
    internal static void UpdatePages(Chapter chapter)
    {
      chapter.Pages.Clear();
      var document = new HtmlDocument();
      document.LoadHtml(Page.GetPage(chapter.Uri));

      var firstOrDefault = document.DocumentNode
          .SelectNodes("//div[@class=\"pageBlock container reader-bottom\"]")
          .FirstOrDefault();

      if (firstOrDefault == null)
        return;

      var i = 0;
      var chapterLinksList = Regex
        .Matches(firstOrDefault.OuterHtml, @"{url:""(.*?)""", RegexOptions.IgnoreCase)
        .OfType<Group>()
        .Select(g => g.Captures[0])
        .OfType<Match>()
        .Select(m => m.Groups[1].Value)
        .Select(s => (!Uri.IsWellFormedUriString(s, UriKind.Absolute)) ? (@"http://" + chapter.Uri.Host + s) : s)
        .Select(s => new MangaPage(chapter.Uri, new Uri(s), i++))
        .ToList();
      chapter.Pages.AddRange(chapterLinksList);
    }
  }
}
