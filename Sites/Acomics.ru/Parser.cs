using System;
using System.Collections.Generic;
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
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace Acomics
{
  public class Parser : BaseSiteParser
  {
    private static readonly string VolumeClassName = "serial-chapters-head";
    private static readonly string VolumeXPath = string.Format("//*[@class=\"{0}\"]", VolumeClassName);
    private static readonly string ChapterXPath = "//div[@class=\"chapters\"]//a";

    /// <summary>
    /// Обновить название и статус манги.
    /// </summary>
    /// <param name="manga">Манга.</param>
    public override async Task UpdateNameAndStatus(IManga manga)
    {
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml((await Page.GetPageAsync(new Uri(manga.Uri.OriginalString + @"/about"), AcomicsPlugin.Instance.GetCookieClient(true)).ConfigureAwait(false)).Content);
        var nameNode = document.DocumentNode.SelectSingleNode("//head//meta[@property=\"og:title\"]");
        if (nameNode != null && nameNode.Attributes.Any(a => Equals(a.Name, "content")))
        {
          var name = WebUtility.HtmlDecode(nameNode.Attributes.Single(a => Equals(a.Name, "content")).Value);
          UpdateName(manga, name);
        }

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

    public override async Task UpdateContentType(IManga manga)
    {
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml((await Page.GetPageAsync(new Uri(manga.Uri.OriginalString + @"/content"), AcomicsPlugin.Instance.GetCookieClient(true)).ConfigureAwait(false)).Content);
        manga.HasVolumes = document.DocumentNode.SelectNodes(VolumeXPath) != null;
        manga.HasChapters = document.DocumentNode.SelectNodes(ChapterXPath) != null;
      }
      catch (System.Exception) { }
    }

    /// <summary>
    /// Получить содержание манги - тома и главы.
    /// </summary>
    /// <param name="manga">Манга.</param>
    public override async Task UpdateContent(IManga manga)
    {
      var volumes = new List<VolumeDto>();
      var chapters = new List<ChapterDto>();
      var pages = new List<MangaPageDto>();
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml((await Page.GetPageAsync(new Uri(manga.Uri.OriginalString + @"/content"), AcomicsPlugin.Instance.GetCookieClient(true)).ConfigureAwait(false)).Content);

        var volumeNodes = document.DocumentNode.SelectNodes(VolumeXPath);
        if (volumeNodes != null)
          for (var i = 0; i < volumeNodes.Count; i++)
          {
            var volume = volumeNodes[i];
            var desc = WebUtility.HtmlDecode(volume.InnerText);
            var newVolume = new VolumeDto() { Name = desc, Number = volumes.Count + 1 };
            var skipped = volume.ParentNode.ChildNodes
              .SkipWhile(cn => cn.PreviousSibling != volume);
            var volumeChapterNodes = skipped
              .TakeWhile(cn => !cn.Attributes.Any() || cn.Attributes[0].Value != VolumeClassName);
            var volumeChapters = volumeChapterNodes
              .Select(cn => cn.SelectNodes(".//a"))
              .Where(cn => cn != null)
              .SelectMany(cn => cn)
              .Select(CreateChapterDto);
            newVolume.Container.AddRange(volumeChapters);
            volumes.Add(newVolume);
          }

        if (volumeNodes == null || !volumes.Any())
        {
          var nodes = document.DocumentNode.SelectNodes(ChapterXPath);
          if (nodes != null)
            chapters.AddRange(nodes.Select(CreateChapterDto));
        }

        var allPages = await GetMangaPages(manga.Uri).ConfigureAwait(false);
        var innerChapters = chapters.Count == 0 ? volumes.SelectMany(v => v.Container).ToList() : chapters;
        for (int i = 0; i < innerChapters.Count; i++)
        {
          var current = innerChapters[i].Number;
          var next = i + 1 != innerChapters.Count ? innerChapters[i + 1].Number : int.MaxValue;
          innerChapters[i].Container.AddRange(allPages.Where(p => current <= p.Number && p.Number < next));
          innerChapters[i].Number = i + 1;
        }
        pages = allPages.Except(innerChapters.SelectMany(c => c.Container)).ToList();
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }

      manga.HasVolumes = volumes.Any();
      manga.HasChapters = volumes.Any() || chapters.Any();
      FillMangaVolumes(manga, volumes);
      FillMangaChapters(manga, chapters);
      FillMangaPages(manga, pages);
    }

    public override Task UpdatePages(Chapter chapter)
    {
      // Acomics do that in UpdateContent
      return Task.CompletedTask;
    }

    private static ChapterDto CreateChapterDto(HtmlNode cn)
    {
      var uri = cn.Attributes[0].Value;
      return new ChapterDto(uri, WebUtility.HtmlDecode(cn.Attributes.Count > 1 ? cn.Attributes[1].Value : cn.InnerText))
      {
        // Главе присваивается кривой номер, так и задумано, он будет перебит после заполнения страницами.
        Number = GetChapterNumber(uri)
      };
    }

    internal static int GetChapterNumber(string uri)
    {
      return Convert.ToInt32(Regex.Match(uri, @"/[-]?[0-9]+", RegexOptions.RightToLeft).Value.Remove(0, 1));
    }

    public override UriParseResult ParseUri(Uri uri)
    {
      // Manga : https://acomics.ru/~hotblood
      // Volume : -
      // Chapter : -
      // Page : https://acomics.ru/~hotblood/60

      var hosts = ConfigStorage.Plugins
        .Where(p => p.GetParser().GetType() == typeof(Parser))
        .Select(p => p.GetSettings().MainUri);

      foreach (var host in hosts)
      {
        if (!Equals(uri.Host, host.Host))
          continue;

        if (uri.Segments.Length > 1)
        {
          var mangaUri = new Uri(host, uri.Segments[1].TrimEnd('/'));
          if (uri.Segments.Length == 3)
            return new UriParseResult(true, UriParseKind.Page, mangaUri);
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
        var client = await AcomicsPlugin.Instance.GetCookieClient(true).ConfigureAwait(false);
        document.LoadHtml((await Page.GetPageAsync(new Uri(manga.Uri.OriginalString + @"/banner"), client).ConfigureAwait(false)).Content);
        var banners = document.DocumentNode.SelectSingleNode("//div[@class='serial-content']");
        var image = banners.ChildNodes.SkipWhile(n => n.InnerText != "160x90").Skip(1).FirstOrDefault();
        var src = image.ChildNodes[0].Attributes.Single(a => a.Name == "src").Value;
        Uri link;
        if (Uri.IsWellFormedUriString(src, UriKind.Relative))
          link = new Uri(manga.Setting.MainUri, src);
        else
          link = new Uri(src);
        result = client.DownloadData(link);
      }
      catch (Exception ex) { Log.Exception(ex); }
      return new[] { result };
    }

    protected override async Task<(HtmlNodeCollection Nodes, Uri Uri, CookieClient CookieClient)> GetMangaNodes(string name, Uri host)
    {
      var searchHost = new Uri(host, "search?keyword=" + WebUtility.UrlEncode(name));
      var client = await AcomicsPlugin.Instance.GetCookieClient(true).ConfigureAwait(false);
      var page = await Page.GetPageAsync(searchHost, client).ConfigureAwait(false);
      if (!page.HasContent)
        return (null, null, null);

      return await Task.Run(() =>
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        return (document.DocumentNode.SelectNodes("//table[@class='catalog-elem list-loadable']"), host, client);
      }).ConfigureAwait(false);
    }

    protected override async Task<IManga> GetMangaFromNode(Uri host, CookieClient client, HtmlNode manga)
    {
      var image = manga.SelectSingleNode(".//td[@class='catdata1']//a//img");
      var imageUri = image?.Attributes.Single(a => a.Name == "src").Value;

      var mangaNode = manga.SelectSingleNode(".//div[@class='title']//a");
      var mangaUri = mangaNode.Attributes.Single(a => a.Name == "href").Value;
      var mangaName = mangaNode.InnerText;

      var result = await Mangas.Create(new Uri(host, mangaUri)).ConfigureAwait(false);
      result.Name = WebUtility.HtmlDecode(mangaName);
      if (!string.IsNullOrWhiteSpace(imageUri))
        result.Cover = await client.DownloadDataTaskAsync(new Uri(host, imageUri)).ConfigureAwait(false);
      return result;
    }

    /// <summary>
    /// Получить страницы манги.
    /// </summary>
    /// <param name="uri">Ссылка на мангу.</param>
    /// <returns>Словарь (ссылка, описание).</returns>
    private async Task<List<MangaPageDto>> GetMangaPages(Uri uri)
    {
      var links = new List<Uri>();
      var description = new List<string>();
      var images = new List<Uri>();
      try
      {
        var adultClient = await AcomicsPlugin.Instance.GetCookieClient(true).ConfigureAwait(false);
        var document = new HtmlDocument();
        document.LoadHtml((await Page.GetPageAsync(uri, adultClient).ConfigureAwait(false)).Content);
        var last = document.DocumentNode.SelectSingleNode("//nav[@class='serial']//a[@class='read2']").Attributes[1].Value;
        var count = int.Parse(last.Remove(0, last.LastIndexOf('/') + 1));
        var list = uri.GetLeftPart(UriPartial.Authority) + document.DocumentNode.SelectSingleNode("//nav[@class='serial']//a[@class='read3']").Attributes[1].Value;
        for (var i = 0; i < count; i = i + 5)
        {
          document.LoadHtml((await Page.GetPageAsync(new Uri(list + "?skip=" + i), adultClient).ConfigureAwait(false)).Content);
          foreach (var node in document.DocumentNode.SelectNodes("//div[@class=\"issue\"]//a"))
          {
            links.Add(new Uri(node.Attributes[0].Value));
            var imgNode = node.ChildNodes
              .Single(n => n.Name == "img" && !n.Attributes.Any(a => a.Name == "class" && a.Value == "blank-img"));
            description.Add(imgNode.Attributes.Where(a => a.Name == "alt").Select(a => a.Value).FirstOrDefault());
            images.Add(imgNode.Attributes.Where(a => a.Name == "src").Select(a => new Uri(uri.GetLeftPart(UriPartial.Authority) + a.Value)).Single());
          }
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex, $"Ошибка получения списка глав с адреса {uri}"); }
      catch (ArgumentNullException ex) { Log.Exception(ex, $"Главы не найдены по адресу {uri}"); }

      var pages = new List<MangaPageDto>();
      for (var i = 0; i < links.Count; i++)
      {
        var page = links[i];
        var number = GetChapterNumber(page.OriginalString);
        pages.Add(new MangaPageDto(page, images[i], number, description[i]));
      }
      return pages;
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
