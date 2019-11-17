using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using HtmlAgilityPack;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.DataTrasferObject;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using Newtonsoft.Json.Linq;

namespace Grouple
{
  public abstract class GroupleParser : BaseSiteParser
  {
    /// <summary>
    /// Ключ с куками для редиректа.
    /// </summary>
    internal const string CookieKey = "red-form";

    /// <summary>
    /// Манга удалена.
    /// </summary>
    internal const string Copyright = "Запрещена публикация произведения по копирайту";

    /// <summary>
    /// Получить ссылку с редиректа.
    /// </summary>
    /// <param name="manga">Манга, ссылку которой пытаемся оценить на редирект.</param>
    /// <param name="page">Содержимое страницы по ссылке.</param>
    /// <returns>Новая ссылка.</returns>
    public async Task<Uri> GetRedirectUri(IManga manga, Page page)
    {
      var processedUri = new List<Uri>() { page.ResponseUri };
      while (page.Content.ToLowerInvariant().Contains(CookieKey))
      {
        var redirect = await GetRedirectUriInternal(page, 0).ConfigureAwait(false);
        processedUri.Add(redirect);
        if (processedUri.Count(u => Equals(u, redirect)) > 5)
          throw new GetSiteInfoException($"This webpage has a redirect loop problem (ERR_TOO_MANY_REDIRECTS)", processedUri[0], manga);
        page = await Page.GetPageAsync(redirect, GetClient()).ConfigureAwait(false);
      }

      return page.ResponseUri;
    }

    private async Task<Uri> GetRedirectUriInternal(Page page, int restartCount)
    {
      var fullUri = page.ResponseUri.OriginalString;

      CookieClient client = null;
      try
      {
        client = this.GetClient();
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
        restartCount++;
        if (restartCount > 3)
          return null;

        if (!(await Page.DelayOnExpectationFailed(ex).ConfigureAwait(false)))
          throw;

        return await GetRedirectUriInternal(page, restartCount).ConfigureAwait(false);
      }

      return client?.ResponseUri;
    }

    /// <summary>
    /// Получить ссылки на все изображения в главе.
    /// </summary>
    /// <param name="groupleChapter">Глава.</param>
    /// <returns>Список ссылок на изображения главы.</returns>
    public override async Task UpdatePages(Chapter groupleChapter)
    {
      groupleChapter.Container.Clear();
      var document = new HtmlDocument();
      document.LoadHtml((await Page.GetPageAsync(groupleChapter.Uri, GetClient()).ConfigureAwait(false)).Content);
      var node = document.DocumentNode.SelectNodes("//div[@class=\"pageBlock container reader-bottom\"]").FirstOrDefault();
      if (node == null)
        return;

      var servers = Regex.Match(node.OuterHtml, @"var servers = (\[.*?\])", RegexOptions.IgnoreCase);
      var jsonServers = JToken.Parse(servers.Groups[1].Value).Children().ToList();
      var serversList = jsonServers.Select(server => new Uri(server.ToString())).ToList();

      var initBlock = Regex.Match(node.OuterHtml, @"rm_h\.init\([ ]*(\[\[.*?\]\])", RegexOptions.IgnoreCase);
      var jsonParsed = JToken.Parse(initBlock.Groups[1].Value).Children().ToList();
      for (var i = 0; i < jsonParsed.Count; i++)
      {
        var child = jsonParsed[i];
        var uriString = child[1].ToString() + child[0] + child[2];

        // Фикс страницы с цензурой.
        if (!Uri.TryCreate(uriString, UriKind.Absolute, out Uri imageLink))
          imageLink = new Uri(groupleChapter.Uri.GetLeftPart(UriPartial.Authority) + uriString);

        groupleChapter.Container.Add(new GroupleMangaPage(groupleChapter.Uri, imageLink, i, serversList, groupleChapter));
      }
    }

    public override async Task UpdateNameAndStatus(IManga manga)
    {
      var page = await Page.GetPageAsync(manga.Uri, GetClient()).ConfigureAwait(false);
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

      UpdateName(manga, localizedName.ToString());

      var status = string.Empty;
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        var nodes = document.DocumentNode.SelectNodes("//div[@class=\"subject-meta col-sm-7\"]//p");
        if (nodes != null)
          status = nodes.Aggregate(status, (current, node) =>
            current + Regex.Replace(WebUtility.HtmlDecode(node.InnerText).Trim(), @"\s+", " ").Replace("\n", "") + Environment.NewLine);
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
      manga.Status = status;

      var description = string.Empty;
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        var node = document.DocumentNode.SelectSingleNode("//div[@class=\"manga-description\"]");
        if (node != null)
          description = WebUtility.HtmlDecode(node.InnerText).Trim();
      }
      catch (Exception e) { Log.Exception(e); }
      manga.Description = description;
    }

    public override async Task UpdateContent(IManga manga)
    {
      var dic = new Dictionary<Uri, string>();
      var links = new List<Uri> { };
      var description = new List<string> { };
      var page = await Page.GetPageAsync(manga.Uri, GetClient()).ConfigureAwait(false);
      var hasCopyrightNotice = false;
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        hasCopyrightNotice = document.DocumentNode.InnerText.Contains(Copyright);
        var linkNodes = document.DocumentNode
          .SelectNodes("//div[@class=\"expandable chapters-link\"]//a[@href]")
          .Reverse()
          .ToList();
        links = linkNodes
          .ConvertAll(r => r.Attributes.Single(a => a.Name == "href").Value)
          .Where(k => k != string.Empty)
          .Select(s => page.ResponseUri.GetLeftPart(UriPartial.Authority) + s + "?mature=1")
          .Select(s => new Uri(s))
          .ToList();
        description = linkNodes
          .ConvertAll(r => WebUtility.HtmlDecode(r.InnerText.Replace("\r\n", string.Empty).Trim()))
          .ToList();
      }
      catch (NullReferenceException ex)
      {
        Log.Exception(ex, $"Ошибка получения списка глав с адреса {page.ResponseUri}");
      }
      catch (ArgumentNullException ex)
      {
        Log.Exception(ex, hasCopyrightNotice
            ? $"{Copyright}, адрес манги {page.ResponseUri}"
            : $"Главы не найдены по адресу {page.ResponseUri}");
      }

      for (var i = 0; i < links.Count; i++)
      {
        dic.Add(links[i], description[i]);
      }

      var rmVolumes = dic
        .Select(cs => new GroupleChapter(cs.Key, cs.Value))
        .GroupBy(c => c.VolumeNumber)
        .Select(g =>
        {
          var v = new VolumeDto(g.Key);
          v.Container.AddRange(g.Select(c => new ChapterDto(c.Uri, c.Name) { Number = c.Number }));
          return v;
        })
        .ToList();

      FillMangaVolumes(manga, rmVolumes);
    }

    public override UriParseResult ParseUri(Uri uri)
    {
      // Manga : https://readmanga.me/heroes_of_the_western_world__emerald_
      // Volume : -
      // Chapter : https://readmanga.me/heroes_of_the_western_world__emerald_/vol0/0
      // Page : -

      var hosts = ConfigStorage.Plugins
        .Where(p => p.GetParser() is GroupleParser)
        .Select(p => p.GetSettings().MainUri);

      foreach (var host in hosts)
      {
        if (!Equals(uri.Host, host.Host))
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

    public override Task<IEnumerable<byte[]>> GetPreviews(IManga manga)
    {
      return GetPreviewsImpl(manga);
    }

    protected override async Task<(HtmlNodeCollection Nodes, Uri Uri, CookieClient CookieClient)> GetMangaNodes(string name, Uri host)
    {
      var searchHost = new Uri(host, "search");
      var client = GetClient();
      var page = await client.UploadValuesTaskAsync(searchHost, new NameValueCollection() { { "q", WebUtility.UrlEncode(name) } }).ConfigureAwait(false);
      if (page == null)
        return (null, null, null);

      return await Task.Run(() =>
      {
        var document = new HtmlDocument();
        document.LoadHtml(Encoding.UTF8.GetString(page));
        return (document.DocumentNode.SelectNodes("//div[contains(@class, 'col-sm-6')]"), host, client);
      }).ConfigureAwait(false);
    }

    protected override async Task<IManga> GetMangaFromNode(Uri host, CookieClient client, HtmlNode manga)
    {
      // Это переводчик, идем дальше.
      if (manga.SelectSingleNode(".//i[@class='fa fa-user text-info']") != null)
        return null;

      var image = manga.SelectSingleNode(".//div[@class='img']//a//img");
      var imageUri = image?.Attributes.Single(a => a.Name == "data-original").Value;

      var mangaNode = manga.SelectSingleNode(".//h3//a");
      var mangaUri = mangaNode.Attributes.Single(a => a.Name == "href").Value;
      var mangaName = mangaNode.Attributes.Single(a => a.Name == "title").Value;

      if (!Uri.TryCreate(mangaUri, UriKind.Relative, out Uri test))
        return null;

      var result = await Mangas.Create(new Uri(host, mangaUri)).ConfigureAwait(false);
      result.Name = WebUtility.HtmlDecode(mangaName);
      if (!string.IsNullOrWhiteSpace(imageUri))
        result.Cover = await client.DownloadDataTaskAsync(new Uri(host, imageUri)).ConfigureAwait(false);
      return result;
    }

    private async Task<IEnumerable<byte[]>> GetPreviewsImpl(IManga manga)
    {
      var document = new HtmlDocument();
      var client = this.GetClient();
      document.LoadHtml((await Page.GetPageAsync(manga.Uri, client).ConfigureAwait(false)).Content);
      var banners = document.DocumentNode.SelectSingleNode("//div[@class='picture-fotorama']");
      var images = new List<byte[]>();
      foreach (var node in banners.ChildNodes)
      {
        Uri link = null;
        try
        {
          var attributes = node.Attributes.Where(a => a.Name == "src" || a.Name == "href").ToList();
          if (!attributes.Any())
            continue;

          var src = attributes.Single().Value;
          if (Uri.IsWellFormedUriString(src, UriKind.Relative))
            link = new Uri(manga.Setting.MainUri, src);
          else
            link = new Uri(src);
        }
        catch (NullReferenceException ex) { Log.Exception(ex); }
        if (link == null)
          continue;

        byte[] image = null;
        try
        {
          image = client.DownloadData(link);
        }
        catch (Exception e)
        {
          Log.Exception(e);
        }
        if (image != null)
          images.Add(image);
      }

      return images;
    }
  }
}
