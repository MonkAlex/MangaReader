using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using HtmlAgilityPack;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.DataTrasferObject;
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
    internal const string CookieKey = "red-form";

    /// <summary>
    /// Манга удалена.
    /// </summary>
    internal const string Copyright = "Запрещена публикация произведения по копирайту";

    /// <summary>
    /// Получить ссылку с редиректа.
    /// </summary>
    /// <param name="page">Содержимое страницы по ссылке.</param>
    /// <returns>Новая ссылка.</returns>
    public static Uri GetRedirectUri(Page page)
    {
      return GetRedirectUriInternal(page, 0);
    }

    private static Uri GetRedirectUriInternal(Page page, int restartCount)
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
        restartCount++;
        if (restartCount > 3)
          return null;

        if (!Page.DelayOnExpectationFailed(ex))
          throw;

        return GetRedirectUriInternal(page, restartCount);
      }

      return client?.ResponseUri;
    }

    /// <summary>
    /// Получить ссылки на все изображения в главе.
    /// </summary>
    /// <param name="groupleChapter">Глава.</param>
    /// <returns>Список ссылок на изображения главы.</returns>
    public static void UpdatePages(GroupleChapter groupleChapter)
    {
      groupleChapter.Container.Clear();
      var document = new HtmlDocument();
      document.LoadHtml(Page.GetPage(groupleChapter.Uri).Content);
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
          imageLink = new Uri(@"http://" + groupleChapter.Uri.Host + uriString);

        groupleChapter.Container.Add(new GroupleMangaPage(groupleChapter.Uri, imageLink, i, serversList));
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

      UpdateName(manga, localizedName.ToString());

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
          .ConvertAll(r => r.Attributes.ToList().ConvertAll(i => i.Value))
          .SelectMany(j => j)
          .Where(k => k != string.Empty)
          .Select(s => @"http://" + page.ResponseUri.Host + s + "?mature=1")
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

    public override IEnumerable<byte[]> GetPreviews(IManga manga)
    {
      return GetPreviewsImpl(manga);
    }

    public override IEnumerable<IManga> Search(string name)
    {
      var hosts = ConfigStorage.Plugins
        .Where(p => p.GetParser().GetType() == typeof(Parser))
        .Select(p => p.GetSettings().MainUri);

      var client = new CookieClient();
      foreach (var host in hosts)
      {
        var searchHost = new Uri(host, "search?q=" + WebUtility.UrlEncode(name));
        var page = Page.GetPage(searchHost, client);
        if (!page.HasContent)
          continue;

        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        var mangas = document.DocumentNode.SelectNodes("//div[@class='tile col-sm-6']");
        if (mangas == null)
          continue;

        foreach (var manga in mangas)
        {
          // Это переводчик, идем дальше.
          if (manga.SelectSingleNode(".//i[@class='fa fa-user text-info']") != null)
            continue;

          var image = manga.SelectSingleNode(".//div[@class='img']//a//img");
          var imageUri = image?.Attributes.Single(a => a.Name == "data-original").Value;

          var mangaNode = manga.SelectSingleNode(".//h3//a");
          var mangaUri = mangaNode.Attributes.Single(a => a.Name == "href").Value;
          var mangaName = mangaNode.Attributes.Single(a => a.Name == "title").Value;

          if (!Uri.TryCreate(mangaUri, UriKind.Relative, out Uri test))
            continue;

          var result = Mangas.Create(new Uri(host, mangaUri));
          result.Name = WebUtility.HtmlDecode(mangaName);
          if (imageUri != null)
            result.Cover = client.DownloadData(imageUri);
          yield return result;
        }
      }
    }

    private IEnumerable<byte[]> GetPreviewsImpl(IManga manga)
    {
      var document = new HtmlDocument();
      var client = new CookieClient();
      document.LoadHtml(Page.GetPage(manga.Uri, client).Content);
      var banners = document.DocumentNode.SelectSingleNode("//div[@class='picture-fotorama']");
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
          yield return image;
      }
    }

    public override IMapper GetMapper()
    {
      return Mappers.GetOrAdd(typeof(Parser), type =>
      {
        var config = new MapperConfiguration(cfg =>
        {
          cfg.AddCollectionMappers();
          cfg.CreateMap<VolumeDto, Volume>()
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<ChapterDto, MangaReader.Core.Manga.Chapter>()
            .ConstructUsing(dto => new GroupleChapter(dto.Uri, dto.Name))
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<ChapterDto, GroupleChapter>()
            .IncludeBase<ChapterDto, MangaReader.Core.Manga.Chapter>()
            .ConstructUsing(dto => new GroupleChapter(dto.Uri, dto.Name))
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<MangaPageDto, MangaPage>()
            .EqualityComparison((src, dest) => src.ImageLink == dest.ImageLink);
        });
        return config.CreateMapper();
      });
    }
  }
}
