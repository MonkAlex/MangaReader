using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MangaReader.Services;

namespace MangaReader.Manga.Hentaichan
{
  public static class Getter
  {
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
        document.LoadHtml(Page.GetPage(uri));
        var nameNode = document.DocumentNode.SelectSingleNode("//head/title");
        if (nameNode != null)
          name = nameNode.InnerText.Replace("Все главы", "").Trim().TrimEnd('-').Trim();
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
      return WebUtility.HtmlDecode(name);
    }

    public static void UpdateContent(Hentaichan manga)
    {
      var chapters = new List<Chapter>();
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(Page.GetPage(manga.Uri));

        var chapterNodes = document.DocumentNode.SelectNodes("//div[@class=\"related_info\"]");
        foreach (var node in chapterNodes)
        {
          var link = node.SelectSingleNode(".//h2//a");
          var desc = node.SelectSingleNode(".//div[@class=\"related_tag_list\"]");
          chapters.Add(new Chapter(new Uri(manga.Uri, link.Attributes[0].Value), desc.InnerText));
        }
      }
      catch (NullReferenceException ex)
      {
        Library.Status = "Возможно требуется регистрация";
        Log.Exception(ex, manga.Uri.OriginalString);
      }

      manga.Chapters.AddRange(chapters);
    }

    public static void UpdatePages(Chapter chapter)
    {
      chapter.Pages.Clear();
      var pages = new List<MangaPage>();
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(Page.GetPage(new Uri(chapter.Uri.OriginalString.Replace("/manga/", "/online/"))));

        var imgs = Regex.Match(document.DocumentNode.OuterHtml, @"""(fullimg.*)", RegexOptions.IgnoreCase).Groups[1].Value.Remove(0, 9);
        foreach (Match match in Regex.Matches(imgs, @"""(.*?)"","))
        {
          pages.Add(new MangaPage(chapter.Uri, new Uri(match.Groups[1].Value)));
        }

      }
      catch (NullReferenceException ex) { Log.Exception(ex); }

      chapter.Pages.AddRange(pages);
    }
  }
}
