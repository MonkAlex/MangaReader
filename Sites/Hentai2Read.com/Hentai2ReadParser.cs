using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
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
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using Newtonsoft.Json.Linq;

namespace Hentai2Read.com
{
  public class Hentai2ReadParser : BaseSiteParser
  {
    /// <summary>
    /// Обновить название и статус манги.
    /// </summary>
    /// <param name="manga">Манга.</param>
    public override async Task UpdateNameAndStatus(IManga manga)
    {
      try
      {
        var client = await Hentai2ReadPlugin.Instance.GetCookieClient(true).ConfigureAwait(false);
        var document = new HtmlDocument();
        document.LoadHtml((await client.GetPage(manga.Uri).ConfigureAwait(false)).Content);
        var nameNode = document.DocumentNode.SelectSingleNode("//span[@itemprop=\"name\"]");
        if (nameNode != null)
        {
          var name = WebUtility.HtmlDecode(nameNode.InnerText);
          UpdateName(manga, name);
        }

        var content = document.DocumentNode.SelectSingleNode("//ul[@class=\"list list-simple-mini\"]");
        if (content != null)
        {
          var summary = string.Empty;
          var completed = content.SelectNodes(".//a").Any(n => WebUtility.HtmlDecode(n.InnerText).ToLowerInvariant().Contains("completed"));
          manga.IsCompleted = completed;
          var nodes = content.SelectNodes(".//li[@class=\"text-primary\"]");
          summary = nodes
            .Where(n => !n.InnerText.Contains("Storyline"))
            .Aggregate(summary, GetStatusNodeText);
          manga.Status = summary;

          var description = nodes
            .FirstOrDefault(n => n.InnerText.Contains("Storyline") && !n.InnerText.Contains("Nothing yet!"));
          if (description != null)
            manga.Description = description.ChildNodes
              .Aggregate(string.Empty, (current, node) =>
                current + Regex.Replace(WebUtility.HtmlDecode(node.InnerText).Trim(), @"\s+", " ").Replace("\n", "") + Environment.NewLine)
              .Trim();
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
    }

    private static string GetStatusNodeText(string current, HtmlNode node)
    {
      current += WebUtility.HtmlDecode(node.ChildNodes.First(n => n.Name == "b").InnerText).Trim();
      current += "  ";
      current += string.Join(", ", node.ChildNodes.Where(n => n.Name == "a").Select(n => WebUtility.HtmlDecode(n.InnerText).Trim()));
      current += Environment.NewLine;
      return current;
    }

    /// <summary>
    /// Получить содержание манги - главы.
    /// </summary>
    /// <param name="manga">Манга.</param>
    public override async Task UpdateContent(IManga manga)
    {
      var chapters = new List<ChapterDto>();
      try
      {
        var client = await Hentai2ReadPlugin.Instance.GetCookieClient(true).ConfigureAwait(false);
        var document = new HtmlDocument();
        document.LoadHtml((await client.GetPage(manga.Uri).ConfigureAwait(false)).Content);

        var chapterNodes = document.DocumentNode.SelectNodes("//a[@class=\"pull-left font-w600\"]").Reverse();
        foreach (var chapterNode in chapterNodes)
        {
          var uri = chapterNode.Attributes.Single(a => a.Name == "href").Value;
          var text = WebUtility.HtmlDecode(chapterNode.FirstChild.InnerText.Trim());
          var number = Regex.Match(text, "^[0-9\\.]+", RegexOptions.Compiled).Value;
          var parsedNumber = double.Parse(number, NumberStyles.Float, CultureInfo.InvariantCulture);
          chapters.Add(new ChapterDto(uri, text) { Number = parsedNumber });
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }

      FillMangaChapters(manga, chapters);
    }

    public override async Task UpdatePages(Chapter chapter)
    {
      chapter.Container.Clear();
      var pages = new List<MangaPage>();
      try
      {
        var document = new HtmlDocument();
        var client = await Hentai2ReadPlugin.Instance.GetCookieClient(true).ConfigureAwait(false);
        var page = await client.GetPage(chapter.Uri).ConfigureAwait(false);
        document.LoadHtml(page.Content);

        var imgs = Regex.Match(document.DocumentNode.OuterHtml, @"\'images\'\s*:\s*(\[.+\])", RegexOptions.IgnoreCase).Groups[1].Value;
        var jsonParsed = JToken.Parse(imgs).Children().ToList();
        for (var i = 0; i < jsonParsed.Count; i++)
        {
          var uriString = jsonParsed[i].ToString();
          pages.Add(new MangaPage(chapter.Uri, new Uri("https://static.hentaicdn.com/hentai" + uriString), i + 1, chapter));
        }
      }
      catch (Exception ex) { Log.Exception(ex); }

      chapter.Container.AddRange(pages);
    }

    public override UriParseResult ParseUri(Uri uri)
    {
      // Manga : https://hentai2read.com/daily_life_with_a_monster_girl/
      // Volume : -
      // Chapter : https://hentai2read.com/daily_life_with_a_monster_girl/60.1/
      // Page : -

      var hosts = ConfigStorage.Plugins
        .Where(p => p.GetParser().GetType() == typeof(Hentai2ReadParser))
        .Select(p => p.GetSettings().MainUri);

      foreach (var host in hosts)
      {
        if (!Equals(uri.Host, host.Host))
          continue;

        if (uri.Segments.Length > 1)
        {
          var mangaUri = new Uri(host, uri.Segments[1]);
          if (uri.Segments.Length == 3)
            return new UriParseResult(true, UriParseKind.Chapter, mangaUri);
          if (uri.Segments.Length == 2)
            return new UriParseResult(true, UriParseKind.Manga, mangaUri);
        }
      }

      return new UriParseResult(false, UriParseKind.Manga, null);
    }

    public override async Task<IEnumerable<byte[]>> GetPreviews(IManga manga)
    {
      byte[] result = null;
      try
      {
        var document = new HtmlDocument();
        var client = await Hentai2ReadPlugin.Instance.GetCookieClient(true).ConfigureAwait(false);
        document.LoadHtml((await client.GetPage(manga.Uri).ConfigureAwait(false)).Content);
        var imageBlock = document.DocumentNode.SelectSingleNode("//img[@class=\"img-responsive border-black-op\"]");
        var src = imageBlock.Attributes.Single(a => a.Name == "src").Value;
        result = await client.GetData(new Uri(src)).ConfigureAwait(false);
      }
      catch (Exception ex) { Log.Exception(ex); }
      return new[] { result };
    }

    protected override async Task<(HtmlNodeCollection Nodes, Uri Uri, ISiteHttpClient CookieClient)> GetMangaNodes(string name, Uri host)
    {
      var searchHost = new Uri(host, "hentai-list/search/");
      var client = await Hentai2ReadPlugin.Instance.GetCookieClient(true).ConfigureAwait(false);
      var page = await client.Post(searchHost, new Dictionary<string, string>()
        { { "cmd_wpm_wgt_mng_sch_sbm", "Search" }, {"txt_wpm_wgt_mng_sch_nme", name}}).ConfigureAwait(false);
      if (page == null)
        return (null, null, null);

      return await Task.Run(() =>
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        return (document.DocumentNode.SelectNodes("//div[@class=\"col-xs-6 col-sm-4 col-md-3 col-xl-2\"]"), host, client);
      }).ConfigureAwait(false);
    }

    public Task<IManga> GetMangaFromBookmarks(Uri host, ISiteHttpClient client, HtmlNode mangaNode)
    {
      return GetMangaFromNode(host, client, mangaNode);
    }

    protected override async Task<IManga> GetMangaFromNode(Uri host, ISiteHttpClient client, HtmlNode manga)
    {
      var image = manga.SelectSingleNode(".//img");
      var imageUri = image?.Attributes.Single(a => a.Name == "data-src").Value;

      var mangaNode = manga.SelectSingleNode(".//div[@class='overlay-title']//a");
      var mangaUri = mangaNode.Attributes.Single(a => a.Name == "href").Value;
      var mangaName = mangaNode.InnerText.Trim();

      var result = await Mangas.Create(new Uri(host, mangaUri)).ConfigureAwait(false);
      result.Name = WebUtility.HtmlDecode(mangaName);
      if (!string.IsNullOrWhiteSpace(imageUri) && client != null)
        result.Cover = await client.GetData(new Uri(host, imageUri)).ConfigureAwait(false);
      return result;
    }

    public override IMapper GetMapper()
    {
      return Mappers.GetOrAdd(typeof(Hentai2ReadParser), type =>
      {
        var config = new MapperConfiguration(cfg =>
        {
          cfg.AddCollectionMappers();
          cfg.CreateMap<VolumeDto, Volume>()
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<ChapterDto, Chapter>()
            .ConstructUsing(dto => new Chapter(dto.Uri, dto.Name))
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<MangaPageDto, MangaPage>()
            .EqualityComparison((src, dest) => src.ImageLink == dest.ImageLink);
        });
        return config.CreateMapper();
      });
    }
  }
}
