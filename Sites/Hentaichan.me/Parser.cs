using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
    private static readonly string AdultOnly = "Доступ ограничен. Только зарегистрированные пользователи подтвердившие, что им 18 лет.";

    public static CookieClient GetClient()
    {
      var setting = ConfigStorage.GetPlugin<Hentaichan>().GetSettings();
      var client = new CookieClient();
      if (setting != null)
      {
        var login = setting.Login as HentaichanLogin;
        if (login == null || !login.CanLogin || string.IsNullOrWhiteSpace(login.UserId))
        {
          if (login == null)
          {
            login = new HentaichanLogin() { Name = setting.Login.Name, Password = setting.Login.Password };
            setting.Login = login;
          }

          login.DoLogin().Wait();
        }
        if (!string.IsNullOrWhiteSpace(login.UserId))
        {
          var host = Generic.GetLoginMainUri<Hentaichan>().Host;
          client.Cookie.Add(new Cookie("dle_user_id", login.UserId, "/", host));
          client.Cookie.Add(new Cookie("dle_password", login.PasswordHash, "/", host));
        }
      }
      return client;
    }

    public override void UpdateNameAndStatus(IManga manga)
    {
      var name = string.Empty;
      try
      {
        var document = new HtmlDocument();
        var page = Page.GetPage(manga.Uri);
        document.LoadHtml(page.Content);
        var nameNode = document.DocumentNode.SelectSingleNode("//head/title");
        string[] subString = { "Все главы", "Все части" };
        if (nameNode != null && subString.Any(s => nameNode.InnerText.Contains(s)))
        {
          name = subString
            .Aggregate(nameNode.InnerText, (current, s) => current.Replace(s, string.Empty))
            .Trim().TrimEnd('-').Trim()
            .Replace("\\'", "'");
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
      name = WebUtility.HtmlDecode(name);

      UpdateName(manga, name);
    }

    public override void UpdateContent(IManga manga)
    {
      var chapters = new List<ChapterDto>();
      try
      {
        var document = new HtmlDocument();
        var pages = new List<Uri>() { manga.Uri };
        for (int i = 0; i < pages.Count; i++)
        {
          var content = Page.GetPage(pages[i], GetClient()).Content;
          if (content.Contains(AdultOnly))
            throw new GetSiteInfoException(AdultOnly, manga);
          document.LoadHtml(content);

          // Посчитать странички.
          if (i == 0)
          {
            var pageNodes = document.DocumentNode.SelectNodes("//div[@id=\"pagination_related\"]//a");
            if (pageNodes != null)
            {
              foreach (var node in pageNodes)
              {
                pages.Add(new Uri(manga.Uri, manga.Uri.AbsolutePath + node.Attributes[0].Value));
              }
              pages = pages.Distinct().ToList();
            }
          }

          var chapterNodes = document.DocumentNode.SelectNodes("//div[@class=\"related_info\"]");
          if (chapterNodes != null)
          {
            foreach (var node in chapterNodes)
            {
              var link = node.SelectSingleNode(".//h2//a");
              var desc = node.SelectSingleNode(".//div[@class=\"related_tag_list\"]");
              var uri = new Uri(manga.Uri, link.Attributes[0].Value);
              chapters.Add(new ChapterDto(uri, desc.InnerText) { Number = HentaichanChapter.GetChapterNumber(uri) });
            }
          }
        }
      }
      catch (NullReferenceException ex)
      {
        Log.Exception(ex, $"Возможно, требуется регистрация для доступа к {manga.Uri}");
      }
      catch (GetSiteInfoException ex)
      {
        Log.Exception(ex, string.Format("{0}. {1}", manga.Name, AdultOnly));
      }

      FillMangaChapters(manga, chapters);
    }

    public override UriParseResult ParseUri(Uri uri)
    {
      // Manga : http://henchan.me/related/14212-love-and-devil-glava-25.html
      // Volume : -
      // Chapter : http://henchan.me/online/14212-love-and-devil-glava-25.html
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
        var related = "/related/";
        if (relativeUri.Contains(related))
          return new UriParseResult(true, UriParseKind.Chapter, uri);
        var online = "/online/";
        if (relativeUri.Contains(online))
          return new UriParseResult(true, UriParseKind.Chapter, new Uri(uri, relativeUri.Replace(online, related)));
      }

      return new UriParseResult(false, UriParseKind.Manga, null);
    }

    public override IEnumerable<byte[]> GetPreviews(IManga manga)
    {
      return Mangachan.Parser.GetPreviewsImpl(manga);
    }

    public override IEnumerable<IManga> Search(string name)
    {
      var hosts = ConfigStorage.Plugins
        .Where(p => p.GetParser().GetType() == typeof(Parser))
        .Select(p => p.GetSettings().MainUri);

      var client = new CookieClient();
      foreach (var host in hosts)
      {
        var searchHost = new Uri(host, "?do=search&subaction=search&story=" + WebUtility.UrlEncode(name));
        var page = Page.GetPage(searchHost, client);
        if (!page.HasContent)
          continue;

        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        var mangas = document.DocumentNode.SelectNodes("//div[@class='content_row']");
        if (mangas == null)
          continue;

        foreach (var manga in mangas)
        {
          var image = manga.SelectSingleNode(".//div[@class='manga_images']//img");
          var imageUri = image?.Attributes.Single(a => a.Name == "src").Value;

          var mangaNode = manga.SelectSingleNode(".//h2//a");
          var mangaUri = mangaNode.Attributes.Single(a => a.Name == "href").Value;
          var mangaName = mangaNode.InnerText;

          // Это не манга, идем дальше.
          if (!mangaUri.Contains("/manga/"))
            continue;

          var result = Mangas.Create(new Uri(mangaUri));
          result.Name = WebUtility.HtmlDecode(mangaName);
          if (imageUri != null)
            result.Cover = client.DownloadData(new Uri(host, imageUri));
          yield return result;
        }
      }
    }

    public static void UpdatePages(MangaReader.Core.Manga.Chapter chapter)
    {
      chapter.Container.Clear();
      var pages = new List<MangaPage>();
      try
      {
        var document = new HtmlDocument();
        var page = Page.GetPage(new Uri(chapter.Uri.OriginalString.Replace("/manga/", "/online/")), GetClient());
        document.LoadHtml(page.Content);

        var imgs = Regex.Match(document.DocumentNode.OuterHtml, @"""(fullimg.*)", RegexOptions.IgnoreCase).Groups[1].Value.Remove(0, 9);
        var jsonParsed = JToken.Parse(imgs).Children().ToList();
        for (var i = 0; i < jsonParsed.Count; i++)
        {
          var uriString = jsonParsed[i].ToString();
          pages.Add(new MangaPage(chapter.Uri, new Uri(uriString), i + 1));
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }

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
