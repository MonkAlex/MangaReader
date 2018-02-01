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
    }

    public override void UpdateContent(IManga manga)
    {
      var chapters = new List<ChapterDto>();
      try
      {
        var document = new HtmlDocument();
        var page = GetPageWithRedirect(manga.Uri);
        var content = page.Item1.Content;
        var uri = page.Item2;
        if (content.Contains(AdultOnly))
          throw new GetSiteInfoException(AdultOnly, manga);
        if (content.Contains(NeedRegister))
          throw new GetSiteInfoException("Требуется регистрация.", manga);
        document.LoadHtml(content);
        var headerNode = document.GetElementbyId("right");
        if (!headerNode.InnerText.Contains("Похожая манга"))
        {
          var chapterNodes = headerNode.SelectNodes(".//option");
          if (chapterNodes != null)
          {
            foreach (var node in chapterNodes)
            {
              var chapterUri = new Uri(uri, node.Attributes.Single(a => a.Name == "value").Value);
              chapters.Add(new ChapterDto(chapterUri, WebUtility.HtmlDecode(node.NextSibling.InnerText))
              {
                Number = HentaichanChapter.GetChapterNumber(chapterUri)
              });
            }
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

      FillMangaChapters(manga, chapters);
    }

    private static Tuple<Page, Uri> GetPageWithRedirect(Uri uri)
    {
      uri = new Uri(uri.OriginalString.Replace(@"/manga/", @"/online/"));
      var page = Page.GetPage(uri, GetClient());
      if (page.ResponseUri != uri)
      {
        var url = page.ResponseUri.OriginalString;
        if (!url.EndsWith(IsDevelopment))
          url += IsDevelopment;
        uri = new Uri(url);
        page = Page.GetPage(uri, GetClient());
      }

      return new Tuple<Page, Uri>(page, uri);
    }

    public override UriParseResult ParseUri(Uri uri)
    {
      // Manga : http://henchan.me/manga/14212-love-and-devil-glava-25.html
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
        var manga = "/manga/";
        if (relativeUri.Contains(manga))
          return new UriParseResult(true, UriParseKind.Chapter, uri);
        var online = "/online/";
        if (relativeUri.Contains(online))
          return new UriParseResult(true, UriParseKind.Chapter, new Uri(uri, relativeUri.Replace(online, manga)));
      }

      return new UriParseResult(false, UriParseKind.Manga, null);
    }

    public override IEnumerable<byte[]> GetPreviews(IManga manga)
    {
      return Mangachan.Parser.GetPreviewsImpl(manga);
    }

    protected override async Task<Tuple<HtmlNodeCollection, Uri>> GetMangaNodes(string name, Uri host, CookieClient client)
    {
      var searchHost = new Uri(host, "?do=search&subaction=search&story=" + WebUtility.UrlEncode(name));
      var page = await Page.GetPageAsync(searchHost, client);
      if (!page.HasContent)
        return null;

      return await Task.Run(() =>
      {
        var document = new HtmlDocument();
        document.LoadHtml(page.Content);
        return new Tuple<HtmlNodeCollection, Uri>(document.DocumentNode.SelectNodes("//div[@class='content_row']"), host);
      });
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

      var result = Mangas.Create(new Uri(mangaUri));
      result.Name = WebUtility.HtmlDecode(mangaName);
      result.Cover = await client.DownloadDataTaskAsync(new Uri(host, imageUri));
      return result;
    }

    public static void UpdatePages(MangaReader.Core.Manga.Chapter chapter)
    {
      chapter.Container.Clear();
      var pages = new List<MangaPage>();
      try
      {
        var document = new HtmlDocument();
        var page = GetPageWithRedirect(chapter.Uri);
        document.LoadHtml(page.Item1.Content);

        var imgs = Regex.Match(document.DocumentNode.OuterHtml, @"""(fullimg.*)", RegexOptions.IgnoreCase).Groups[1].Value.Remove(0, 9);
        var jsonParsed = JToken.Parse(imgs).Children().ToList();
        for (var i = 0; i < jsonParsed.Count; i++)
        {
          var uriString = jsonParsed[i].ToString();
          pages.Add(new MangaPage(chapter.Uri, new Uri(uriString), i + 1));
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
