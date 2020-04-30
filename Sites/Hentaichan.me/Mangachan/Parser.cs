using System;
using System.Collections.Generic;
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

namespace Hentaichan.Mangachan
{
  public class Parser : BaseSiteParser
  {
    public override CookieClient GetClient()
    {
      var setting = ConfigStorage.GetPlugin<Mangachan>().GetSettings();
      var client = new MangachanClient();
      if (setting != null)
      {
        var login = setting.Login as BaseLogin;
        if (!login.CanLogin || string.IsNullOrWhiteSpace(login.UserId))
        {
          login.DoLogin(MangachanPlugin.Manga).Wait();
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

    public override async Task UpdateNameAndStatus(IManga manga)
    {
      var page = await Page.GetPageAsync(manga.Uri, GetClient()).ConfigureAwait(false);
      var localizedName = new MangaName();
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        var enName = Regex.Match(document.DocumentNode.InnerHtml, @"title>(.*?) &raquo", RegexOptions.IgnoreCase);
        if (enName.Success)
        {
          localizedName.English = WebUtility.HtmlDecode(enName.Groups[1].Value);
          var regexed = Regex.Escape(localizedName.English);
          var node = document.DocumentNode.SelectSingleNode("//a[@class='title_top_a']");
          var ruName = Regex.Match(node.InnerHtml, $"{regexed} \\((.*?)\\)", RegexOptions.IgnoreCase);
          if (ruName.Success)
            localizedName.Russian = WebUtility.HtmlDecode(ruName.Groups[1].Value);
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }

      UpdateName(manga, localizedName.ToString());

      var status = string.Empty;
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        var nodes = document.DocumentNode.SelectNodes("//table[@class=\"mangatitle\"]//tr");
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
          description = WebUtility.HtmlDecode(node.InnerText).Trim().Replace("Прислать описание", "").TrimEnd();
      }
      catch (Exception e) { Log.Exception(e); }
      manga.Description = description;
    }

    public override async Task UpdateContent(IManga manga)
    {
      var chapters = new List<MangachanChapter>();
      try
      {
        var document = new HtmlDocument();
        var content = (await Page.GetPageAsync(manga.Uri, GetClient()).ConfigureAwait(false)).Content;
        document.LoadHtml(content);

        var chapterNodes = document.DocumentNode.SelectNodes("//table[@class=\"table_cha\"]//a");
        if (chapterNodes != null)
        {
          foreach (var node in chapterNodes)
          {
            var link = node.Attributes.Single(a => a.Name == "href").Value;
            chapters.Add(new MangachanChapter(new Uri(manga.Uri, link), WebUtility.HtmlDecode(node.InnerText)));
          }
          chapters.Reverse();
        }
      }
      catch (NullReferenceException ex)
      {
        Log.Exception(ex, $"Возможно, требуется регистрация для доступа к {manga.Uri}");
      }

      var volumes = new List<VolumeDto>();
      foreach (var volume in chapters.GroupBy(c => c.VolumeNumber).ToList())
      {
        var vol = new VolumeDto(volume.Key);
        vol.Container.AddRange(volume.Select(c => new ChapterDto(c.Uri, c.Name) { Number = c.Number }));
        chapters.RemoveAll(c => volume.Contains(c));
        volumes.Add(vol);
      }

      var chaptersDto = chapters.Select(c => new ChapterDto(c.Uri, c.Name) { Number = c.Number }).ToList();
      FillMangaVolumes(manga, volumes);
      FillMangaChapters(manga, chaptersDto);
    }

    public override UriParseResult ParseUri(Uri uri)
    {
      // Manga : https://manga-chan.me/manga/31910-jigoku-koi-sutefu.html
      // Volume : -
      // Chapter : https://manga-chan.me/online/249080-jigoku-koi-sutefu_v1_ch6.5.html
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
          return new UriParseResult(true, UriParseKind.Chapter, uri);
      }

      return new UriParseResult(false, UriParseKind.Manga, null);
    }

    public override Task<IEnumerable<byte[]>> GetPreviews(IManga manga)
    {
      return GetPreviewsImpl(this, manga);
    }

    protected override async Task<(HtmlNodeCollection Nodes, Uri Uri, CookieClient CookieClient)> GetMangaNodes(string name, Uri host)
    {
      var searchHost = new Uri(host, "?do=search&subaction=search&story=" + WebUtility.UrlEncode(name));
      var client = GetClient();
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

      var result = await Mangas.Create(new Uri(mangaUri)).ConfigureAwait(false);
      result.Name = WebUtility.HtmlDecode(mangaName);
      if (!string.IsNullOrWhiteSpace(imageUri))
        result.Cover = await client.DownloadDataTaskAsync(new Uri(host, imageUri)).ConfigureAwait(false);
      return result;
    }

    internal static async Task<IEnumerable<byte[]>> GetPreviewsImpl(ISiteParser parser, IManga manga)
    {
      var links = new List<Uri>();
      var client = parser.GetClient();
      try
      {
        var document = new HtmlDocument();
        var content = (await Page.GetPageAsync(manga.Uri, client).ConfigureAwait(false)).Content;
        document.LoadHtml(content);

        var chapterNodes = document.DocumentNode.SelectNodes("//img[@id='cover']");
        if (chapterNodes != null)
        {
          foreach (var node in chapterNodes.SelectMany(n => n.Attributes).Where(a => a.Name == "src" && !string.IsNullOrWhiteSpace(a.Value)))
          {
            var src = node.Value;
            Uri link;
            if (Uri.IsWellFormedUriString(src, UriKind.Relative))
              link = new Uri(manga.Setting.MainUri, src);
            else
              link = new Uri(src);
            links.Add(link);
          }
        }
      }
      catch (NullReferenceException ex)
      {
        Log.Exception(ex, $"Возможно, требуется регистрация для доступа к {manga.Uri}");
      }

      var images = new List<byte[]>();
      foreach (var link in links)
      {
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

    public override async Task UpdatePages(MangaReader.Core.Manga.Chapter chapter)
    {
      chapter.Container.Clear();
      var pages = new List<MangaPage>();
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml((await Page.GetPageAsync(chapter.Uri, GetClient()).ConfigureAwait(false)).Content);

        var i = 0;
        var imgs = Regex.Match(document.DocumentNode.OuterHtml, @"""(fullimg.*)", RegexOptions.IgnoreCase).Groups[1].Value.Remove(0, 9);
        foreach (Match match in Regex.Matches(imgs, @"""(.*?)"","))
        {
          pages.Add(new MangaPage(chapter.Uri, new Uri(match.Groups[1].Value), i++, chapter));
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
            .ConstructUsing(dto => new MangachanChapter(dto.Uri, dto.Name))
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<ChapterDto, MangachanChapter>()
            .IncludeBase<ChapterDto, MangaReader.Core.Manga.Chapter>()
            .ConstructUsing(dto => new MangachanChapter(dto.Uri, dto.Name))
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<MangaPageDto, MangaPage>()
            .EqualityComparison((src, dest) => src.ImageLink == dest.ImageLink);
        });
        return config.CreateMapper();
      });
    }
  }
}
