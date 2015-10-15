using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using MangaReader.Account;
using MangaReader.Services;

namespace MangaReader.Manga.Acomic
{
  public static class Getter
  {

    public static CookieClient GetAdultClient()
    {
      var client = new CookieClient();
      client.Cookie.Add(new Cookie("ageRestrict", "40", "/", "acomics.ru"));
      return client;
    }

    /// <summary>
    /// Получить название манги.
    /// </summary>
    /// <param name="uri">Ссылка на мангу.</param>
    /// <returns>Название манги.</returns>
    public static string GetMangaName(Uri uri)
    {
      var name = string.Empty;
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(Page.GetPage(new Uri(uri.OriginalString + @"/about"), Getter.GetAdultClient()));
        var nameNode = document.DocumentNode.SelectSingleNode("//head//meta[@property=\"og:title\"]");
        if (nameNode != null && nameNode.Attributes.Any(a => Equals(a.Name, "content")))
          name = nameNode.Attributes.Single(a => Equals(a.Name, "content")).Value;
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
      return WebUtility.HtmlDecode(name);
    }

    public static void UpdateContentType(Acomics manga)
    {
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(Page.GetPage(new Uri(manga.Uri.OriginalString + @"/content"), Getter.GetAdultClient()));
        manga.HasVolumes = document.DocumentNode.SelectNodes("//h2[@class=\"serial-chapters-head\"]") != null;
        manga.HasChapters = document.DocumentNode.SelectNodes("//div[@class=\"chapters\"]//a") != null;
      }
      catch (Exception){}
    }

    /// <summary>
    /// Получить содержание манги - тома и главы.
    /// </summary>
    /// <param name="manga">Манга.</param>
    public static void UpdateContent(Acomics manga)
    {
      var volumes = new List<Volume>();
      var chapters = new List<Chapter>();
      var pages = new List<MangaPage>();
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(Page.GetPage(new Uri(manga.Uri.OriginalString + @"/content"), Getter.GetAdultClient()));

        var volumeNodes = document.DocumentNode.SelectNodes("//h2[@class=\"serial-chapters-head\"]");
        if (volumeNodes != null)
          for (var i = 0; i < volumeNodes.Count; i++)
          {
            var volume = volumeNodes[i];
            var desc = volume.InnerText;
            var newVolume = new Volume(desc, volumes.Count + 1);
            var skipped = volume.ParentNode.ChildNodes
              .SkipWhile(cn => cn.PreviousSibling != volume);
            var volumeChapterNodes = skipped
              .TakeWhile(cn => cn.Name != "h2");
            var volumeChapters = volumeChapterNodes
              .Select(cn => cn.SelectNodes(".//a"))
              .SelectMany(cn => cn)
              .Select(cn => new Chapter(new Uri(cn.Attributes[0].Value), (cn.Attributes.Count > 1 ? cn.Attributes[1].Value : cn.InnerText)));
            newVolume.Chapters.AddRange(volumeChapters);
            volumes.Add(newVolume);
          }

        if (volumeNodes == null || !volumes.Any())
        {
          var nodes = document.DocumentNode.SelectNodes("//div[@class=\"chapters\"]//a");
          if (nodes != null)
            chapters.AddRange(nodes.Select(cn => new Chapter(new Uri(cn.Attributes[0].Value), (cn.Attributes.Count > 1 ? cn.Attributes[1].Value : cn.InnerText))));
        }

        var allPages = GetMangaPages(manga.Uri);
        var innerChapters = chapters.Count == 0 ? volumes.SelectMany(v => v.Chapters).Cast<Chapter>().ToList() : chapters;
        for (int i = 0; i < innerChapters.Count; i++)
        {
          var current = innerChapters[i].Number;
          var next = i + 1 != innerChapters.Count ? innerChapters[i + 1].Number : int.MaxValue;
          innerChapters[i].Pages.AddRange(allPages.Where(p => current <= p.Number && p.Number < next));
          innerChapters[i].Number = i + 1;
        }
        pages.AddRange(allPages.Except(innerChapters.SelectMany(c => c.Pages).Cast<MangaPage>()));
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }

      manga.HasVolumes = volumes.Any();
      manga.HasChapters = volumes.Any() || chapters.Any();
      manga.Volumes.AddRange(volumes);
      manga.Chapters.AddRange(chapters);
      manga.Pages.AddRange(pages);
    }

    /// <summary>
    /// Получить страницы манги.
    /// </summary>
    /// <param name="uri">Ссылка на мангу.</param>
    /// <returns>Словарь (ссылка, описание).</returns>
    private static List<MangaPage> GetMangaPages(Uri uri)
    {
      var links = new List<Uri>();
      var description = new List<string>();
      var images = new List<Uri>();
      try
      {
        var adultClient = Getter.GetAdultClient();
        var document = new HtmlDocument();
        document.LoadHtml(Page.GetPage(uri, adultClient));
        var last = document.DocumentNode.SelectSingleNode("//nav[@class='serial']//a[@class='read2']").Attributes[1].Value;
        var count = int.Parse(last.Remove(0, last.LastIndexOf('/') + 1));
        var list = @"http://" + uri.Host + document.DocumentNode.SelectSingleNode("//nav[@class='serial']//a[@class='read3']").Attributes[1].Value;
        for (var i = 0; i < count; i = i + 5)
        {
          document.LoadHtml(Page.GetPage(new Uri(list + "?skip=" + i), adultClient));
          links.AddRange(document.DocumentNode
              .SelectNodes("//div[@class=\"issue\"]//a")
              .Select(r => new Uri(r.Attributes[0].Value))
              .ToList());
          description.AddRange(document.DocumentNode
              .SelectNodes("//div[@class=\"issue\"]//img")
              .Select(r => r.Attributes[1].Value)
              .ToList());
          images.AddRange(document.DocumentNode
              .SelectNodes("//div[@class=\"issue\"]//img")
              .Select(r => new Uri(@"http://" + uri.Host + r.Attributes[0].Value))
              .ToList());
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex, "Ошибка получения списка глав.", uri.ToString()); }
      catch (ArgumentNullException ex) { Log.Exception(ex, "Главы не найдены.", uri.ToString()); }

      return links.Select((t, i) => new MangaPage(t, images[i]) { Name = description[i] }).ToList();
    }
  }
}
