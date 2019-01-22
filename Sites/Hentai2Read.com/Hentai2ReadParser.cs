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
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace Hentai2Read.com
{
  public class Hentai2ReadParser : BaseSiteParser
  {
    /// <summary>
    /// Обновить название и статус манги.
    /// </summary>
    /// <param name="manga">Манга.</param>
    public override void UpdateNameAndStatus(IManga manga)
    {
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(Page.GetPage(manga.Uri).Content);
        var nameNode = document.DocumentNode.SelectSingleNode("//span[@itemprop=\"name\"]");
        if (nameNode != null)
        {
          var name = WebUtility.HtmlDecode(nameNode.InnerText);
          UpdateName(manga, name);
        }

        #warning Нет поддержки статуса и прочего
        var content = document.GetElementbyId("contentMargin");
        if (content != null)
        {
          var summary = string.Empty;
          var status = WebUtility.HtmlDecode(content.SelectSingleNode(".//h2").InnerText).ToLowerInvariant().Contains("(закончен)");
          manga.IsCompleted = status;
          var nodes = content.SelectNodes(".//div[@class=\"about-summary\"]//p");
          summary = nodes.Aggregate(summary, (current, node) =>
            current + Regex.Replace(WebUtility.HtmlDecode(node.InnerText).Trim(), @"\s+", " ").Replace("\n", "") + Environment.NewLine);
          manga.Status = summary;

          var description = content.SelectSingleNode(".//div[@class=\"about-summary\"]");
          if (description != null)
            manga.Description = description.ChildNodes
              .SkipWhile(n => n.Name != "p")
              .Skip(1)
              .TakeWhile(n => n.Name != "p")
              .Aggregate(string.Empty, (current, node) => 
                current + Regex.Replace(WebUtility.HtmlDecode(node.InnerText).Trim(), @"\s+", " ").Replace("\n", "") + Environment.NewLine)
              .Trim();
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
    }

    /// <summary>
    /// Получить содержание манги - главы.
    /// </summary>
    /// <param name="manga">Манга.</param>
    public override void UpdateContent(IManga manga)
    {
      var chapters = new List<ChapterDto>();
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(Page.GetPage(manga.Uri).Content);

        var chapterNodes = document.DocumentNode.SelectNodes("//a[@class=\"pull-left font-w600\"]");
        foreach (var chapterNode in chapterNodes)
        {
          var uri = chapterNode.Attributes.Single(a => a.Name == "href").Value;
          var text = chapterNode.InnerText;
          var number = Regex.Match(text, "^[0-9\\.]+", RegexOptions.Compiled).Value;
          chapters.Add(new ChapterDto(uri, text){Number = double.Parse(number)});
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }

      FillMangaChapters(manga, chapters);
    }

    public override UriParseResult ParseUri(Uri uri)
    {
      // Manga : https://hentai2read.com/daily_life_with_a_monster_girl/
      // Volume : -
      // Chapter : https://hentai2read.com/daily_life_with_a_monster_girl/60.1/
      // Page : https://hentai2read.com/daily_life_with_a_monster_girl/60.1/5/

      var hosts = ConfigStorage.Plugins
        .Where(p => p.GetParser().GetType() == typeof(Hentai2ReadParser))
        .Select(p => p.GetSettings().MainUri);

      foreach (var host in hosts)
      {
        var trimmedHost = host.OriginalString;
        if (!uri.OriginalString.StartsWith(trimmedHost))
          continue;

        if (uri.Segments.Length > 1)
        {
          var mangaUri = new Uri(host, uri.Segments[1].TrimEnd('/'));
          if (uri.Segments.Length == 4)
            return new UriParseResult(true, UriParseKind.Page, mangaUri);
          if (uri.Segments.Length == 3)
            return new UriParseResult(true, UriParseKind.Chapter, mangaUri);
          if (uri.Segments.Length == 2)
            return new UriParseResult(true, UriParseKind.Manga, mangaUri);
        }
      }

      return new UriParseResult(false, UriParseKind.Manga, null);
    }

    public override IEnumerable<byte[]> GetPreviews(IManga manga)
    {
      byte[] result = null;
      try
      {
        var document = new HtmlDocument();
        var client = new CookieClient();
        document.LoadHtml(Page.GetPage(manga.Uri, client).Content);
        var imageBlock = document.DocumentNode.SelectSingleNode("//img[@class=\"img-responsive border-black-op\"]");
        var src = imageBlock.Attributes.Single(a => a.Name == "src").Value;
        result = client.DownloadData(src);
      }
      catch (Exception ex) { Log.Exception(ex); }
      yield return result;
    }

    protected override async Task<Tuple<HtmlNodeCollection, Uri>> GetMangaNodes(string name, Uri host, CookieClient client)
    {
      var searchHost = new Uri(host, "hentai-list/search/");
      var page = await client.UploadValuesTaskAsync(searchHost, new NameValueCollection() 
        { { "cmd_wpm_wgt_mng_sch_sbm", "Search" }, {"txt_wpm_wgt_mng_sch_nme", WebUtility.UrlEncode(name)}});
      if (page == null)
        return null;

      return await Task.Run(() =>
      {
        var document = new HtmlDocument();
        document.LoadHtml(Encoding.UTF8.GetString(page));
        return new Tuple<HtmlNodeCollection, Uri>(document.DocumentNode.SelectNodes("//div[@class=\"col-xs-6 col-sm-4 col-md-3 col-lg-2b col-xl-2\"]"), host);
      });
    }

    protected override async Task<IManga> GetMangaFromNode(Uri host, CookieClient client, HtmlNode manga)
    {
      var image = manga.SelectSingleNode(".//img");
      var imageUri = image?.Attributes.Single(a => a.Name == "data-src").Value;

      var mangaNode = manga.SelectSingleNode(".//div[@class='img-overlay text-center']//a");
      var mangaUri = mangaNode.Attributes.Single(a => a.Name == "href").Value;
      var mangaName = mangaNode.InnerText.Trim();

      var result = Mangas.Create(new Uri(host, mangaUri));
      result.Name = WebUtility.HtmlDecode(mangaName);
      if (!string.IsNullOrWhiteSpace(imageUri))
        result.Cover = await client.DownloadDataTaskAsync(new Uri(host, imageUri));
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
