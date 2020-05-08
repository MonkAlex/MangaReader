using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
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

namespace Hentaichan
{
  public class Parser : BaseSiteParser
  {
    private const string AdultOnly = "Доступ ограничен. Только зарегистрированные пользователи подтвердившие, что им 18 лет.";
    private const string NeedRegister = "Контент запрещен на территории РФ";
    private const string IsDevelopment = "?development_access=true";

    public override async Task UpdateNameAndStatus(IManga manga)
    {
      var page = await Page.GetPageAsync(manga.Uri, HentaichanPlugin.Instance.GetCookieClient()).ConfigureAwait(false);
      var name = string.Empty;
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        var nameNode = document.DocumentNode.SelectSingleNode("//head/title");
        var splitter = "&raquo;";
        if (nameNode != null && nameNode.InnerText.Contains(splitter))
        {
          var index = nameNode.InnerText.IndexOf(splitter, StringComparison.InvariantCultureIgnoreCase);
          name = nameNode.InnerText.Substring(0, index);
          index = name.LastIndexOf('-');
          if (index > 0)
          {
            var rightPart = name.Substring(index).ToLowerInvariant();
            if (rightPart.Contains("глава") || rightPart.Contains("часть"))
              name = name.Substring(0, index);
          }
          name = name.Trim();
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
      name = WebUtility.HtmlDecode(name);

      UpdateName(manga, name);

      var status = string.Empty;
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        var nodes = document.DocumentNode.SelectNodes("//div[@id=\"info_wrap\"]//div[@class=\"row\"]");
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
        var node = document.DocumentNode.SelectSingleNode("//div[@id=\"description\"]");
        if (node != null)
          description = WebUtility.HtmlDecode(node.InnerText).Trim();
      }
      catch (Exception e) { Log.Exception(e); }
      manga.Description = description;
    }

    public override async Task UpdateContent(IManga manga)
    {
      var chapters = new List<ChapterDto>();
      try
      {
        var document = new HtmlDocument();
        var mainPage = await Page.GetPageAsync(manga.Uri, HentaichanPlugin.Instance.GetCookieClient()).ConfigureAwait(false);
        var page = await GetPageWithRedirect(manga.Uri).ConfigureAwait(false);
        var content = page.Item1.Content;
        var uri = page.Item2;
        if (content.Contains(AdultOnly))
          throw new GetSiteInfoException(AdultOnly, manga);
        if (content.Contains(NeedRegister))
          throw new GetSiteInfoException("Требуется регистрация.", manga);
        document.LoadHtml(content);
        var headerNode = document.GetElementbyId("right");

        var mainDocument = new HtmlDocument();
        mainDocument.LoadHtml(mainPage.Content);
        var navigationHeader = mainDocument.DocumentNode.SelectSingleNode("//div[@class=\"extaraNavi\"]");
        if (!navigationHeader.InnerText.Contains("Похожая манга"))
        {
          var chapterNodes = headerNode.SelectNodes(".//option");
          if (chapterNodes != null)
          {
            foreach (var node in chapterNodes)
            {
              var chapterUri = new Uri(uri, node.Attributes.Single(a => a.Name == "value").Value);
              chapters.Add(new ChapterDto(chapterUri, WebUtility.HtmlDecode((node.NextSibling ?? node).InnerText))
              {
                Number = HentaichanChapter.GetChapterNumber(chapterUri)
              });
            }
          }
          else
          {
            // Раз не нашли других глав - качаем хотя бы эту.
            chapters.Add(new ChapterDto(uri, manga.ServerName) { Number = 0 });
          }
        }
        else
        {
          // Это манга из одной главы.
          chapters.Add(new ChapterDto(uri, manga.ServerName) { Number = 0 });
        }
      }
      catch (NullReferenceException ex)
      {
        Log.Exception(ex, $"Возможно, требуется регистрация для доступа к {manga.Uri}");
      }
      catch (GetSiteInfoException ex)
      {
        Log.Exception(ex);
      }

      var doubles = chapters.GroupBy(c => c.Number).Where(g => g.Count() > 1).ToList();
      foreach (var groupByNumber in doubles)
      {
        var currentNumber = groupByNumber.Key.ToString(CultureInfo.InvariantCulture);
        if (!currentNumber.Contains('.'))
          currentNumber += '.';
        for (int i = 1; i <= groupByNumber.Count(); i++)
        {
          var chapter = groupByNumber.ElementAt(i - 1);
          chapter.Number = double.Parse(currentNumber + i, NumberStyles.Float, CultureInfo.InvariantCulture);
        }
      }

      FillMangaChapters(manga, chapters);
    }

    private async Task<Tuple<Page, Uri>> GetPageWithRedirect(Uri uri)
    {
      var client = HentaichanPlugin.Instance.GetCookieClient();
      uri = new Uri(uri.OriginalString.Replace(@"/manga/", @"/online/"));
      var page = await Page.GetPageAsync(uri, client).ConfigureAwait(false);
      if (page.ResponseUri != uri)
      {
        var url = page.ResponseUri.OriginalString;
        if (!url.EndsWith(IsDevelopment))
          url += IsDevelopment;
        uri = new Uri(url);
        page = await Page.GetPageAsync(uri, client).ConfigureAwait(false);
      }

      return new Tuple<Page, Uri>(page, uri);
    }

    public override UriParseResult ParseUri(Uri uri)
    {
      // Manga : https://henchan.pro/manga/14212-love-and-devil-glava-25.html
      // Volume : -
      // Chapter : https://henchan.pro/online/14212-love-and-devil-glava-25.html
      // Page : -

      var hosts = ConfigStorage.Plugins
        .Where(p => p.GetParser().GetType() == typeof(Parser))
        .Select(p => p.GetSettings().MainUri);

      foreach (var host in hosts)
      {
        if (!Equals(uri.Host, host.Host))
          continue;

        var uriOriginalString = uri.OriginalString;
        var manga = "/manga/";
        if (uriOriginalString.Contains(manga))
          return new UriParseResult(true, UriParseKind.Manga, uri);
        var online = "/online/";
        if (uriOriginalString.Contains(online))
          return new UriParseResult(true, UriParseKind.Chapter, new Uri(uri, uriOriginalString.Replace(online, manga)));
      }

      return new UriParseResult(false, UriParseKind.Manga, null);
    }

    public override Task<IEnumerable<byte[]>> GetPreviews(IManga manga)
    {
      return Mangachan.Parser.GetPreviewsImpl(this, manga);
    }

    protected override async Task<(HtmlNodeCollection Nodes, Uri Uri, CookieClient CookieClient)> GetMangaNodes(string name, Uri host)
    {
      var searchHost = new Uri(host, "?do=search&subaction=search&story=" + WebUtility.UrlEncode(name));
      var client = HentaichanPlugin.Instance.GetCookieClient();
      var page = await Page.GetPageAsync(searchHost, client).ConfigureAwait(false);
      if (!page.HasContent)
        return (null, null, null);

      return await Task.Run(() =>
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        return (document.DocumentNode.SelectNodes("//div[@class='content_row']"), host, client);
      }).ConfigureAwait(false);
    }

    protected override async Task<IManga> GetMangaFromNode(Uri host, CookieClient client, HtmlNode manga)
    {
      var image = manga.SelectSingleNode(".//div[@class='manga_images']//img");
      var imageUri = image?.Attributes.Single(a => a.Name == "src").Value;

      var mangaNode = manga.SelectSingleNode(".//h2//a");
      var mangaUri = mangaNode.Attributes.Single(a => a.Name == "href").Value;
      var mangaName = mangaNode.InnerText;

      // Это не манга, идем дальше.
      if (!mangaUri.Contains("/manga/"))
        return null;

      var result = await Mangas.Create(new Uri(mangaUri)).ConfigureAwait(false);
      result.Name = WebUtility.HtmlDecode(mangaName);
      if (!string.IsNullOrWhiteSpace(imageUri))
        result.Cover = await client.DownloadDataTaskAsync(new Uri(host, imageUri)).ConfigureAwait(false);
      return result;
    }

    public override async Task UpdatePages(MangaReader.Core.Manga.Chapter chapter)
    {
      chapter.Container.Clear();
      var pages = new List<MangaPage>();
      try
      {
        var document = new HtmlDocument();
        var page = await GetPageWithRedirect(chapter.Uri).ConfigureAwait(false);
        document.LoadHtml(page.Item1.Content);

        var imgs = Regex.Match(document.DocumentNode.OuterHtml, @"""(fullimg.*)", RegexOptions.IgnoreCase).Groups[1].Value.Remove(0, 9);
        var jsonParsed = JToken.Parse(imgs).Children().ToList();
        for (var i = 0; i < jsonParsed.Count; i++)
        {
          var uriString = jsonParsed[i].ToString();
          pages.Add(new MangaPage(chapter.Uri, new Uri(uriString), i + 1, chapter));
        }
      }
      catch (Exception ex) { Log.Exception(ex); }

      chapter.Container.AddRange(pages);
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
            .ConstructUsing(dto => new HentaichanChapter(dto.Uri, dto.Name))
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<ChapterDto, HentaichanChapter>()
            .IncludeBase<ChapterDto, MangaReader.Core.Manga.Chapter>()
            .ConstructUsing(dto => new HentaichanChapter(dto.Uri, dto.Name))
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<MangaPageDto, MangaPage>()
            .EqualityComparison((src, dest) => src.ImageLink == dest.ImageLink);
        });
        return config.CreateMapper();
      });
    }
  }
}
