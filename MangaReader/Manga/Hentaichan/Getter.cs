using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MangaReader.Account;
using MangaReader.Services;
using MangaReader.Services.Config;

namespace MangaReader.Manga.Hentaichan
{
  public static class Getter
  {

    public static CookieClient GetClient()
    {
      var setting = ConfigStorage.Instance.DatabaseConfig.MangaSettings.SingleOrDefault(s => Equals(s.Manga, Hentaichan.Type));
      var client = new CookieClient();
      if (setting != null)
      {
        var login = setting.Login as HentaichanLogin;
        if (login == null || !login.CanLogin || string.IsNullOrWhiteSpace(login.UserId))
        {
          if (login == null)
          {
            login = new HentaichanLogin() {Name = setting.Login.Name, Password = setting.Login.Password};
            setting.Login = login;
          }

          login.DoLogin();
        }
        if (!string.IsNullOrWhiteSpace(login.UserId))
        {
          client.Cookie.Add(new Cookie("dle_user_id", login.UserId, "/", ".hentaichan.ru"));
          client.Cookie.Add(new Cookie("dle_password", login.PasswordHash, "/", ".hentaichan.ru"));
        }
      }
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
        document.LoadHtml(Page.GetPage(uri));
        var nameNode = document.DocumentNode.SelectSingleNode("//head/title");
        string[] subString = {"Все главы", "Все части" };
        if (nameNode != null && subString.Any(s => nameNode.InnerText.Contains(s)))
        {
          name = subString
            .Aggregate(nameNode.InnerText, (current, s) => current.Replace(s, string.Empty))
            .Trim().TrimEnd('-').Trim();
        }
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
        var pages = new List<Uri>() {manga.Uri};
        for (int i = 0; i < pages.Count; i++)
        {
          document.LoadHtml(Page.GetPage(pages[i], GetClient()));

          // Посчитать странички.
          if (i == 0)
          {
            var pageNodes = document.DocumentNode.SelectNodes("//div[@id=\"pagination_related\"]//a");
            if (pageNodes != null)
            {
              foreach (var node in pageNodes)
              {
                pages.Add(new Uri(manga.Uri + node.Attributes[0].Value));
              }
              pages = pages.Distinct().ToList();
            }
          }

          var chapterNodes = document.DocumentNode.SelectNodes("//div[@class=\"related_info\"]");
          foreach (var node in chapterNodes)
          {
            var link = node.SelectSingleNode(".//h2//a");
            var desc = node.SelectSingleNode(".//div[@class=\"related_tag_list\"]");
            chapters.Add(new Chapter(new Uri(manga.Uri, link.Attributes[0].Value), desc.InnerText));
          }
        }
      }
      catch (NullReferenceException ex)
      {
        var status = "Возможно требуется регистрация";
        Library.Status = status;
        Log.Exception(ex, status, manga.Uri.OriginalString);
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
        document.LoadHtml(Page.GetPage(new Uri(chapter.Uri.OriginalString.Replace("/manga/", "/online/")), GetClient()));

        var i = 0;
        var imgs = Regex.Match(document.DocumentNode.OuterHtml, @"""(fullimg.*)", RegexOptions.IgnoreCase).Groups[1].Value.Remove(0, 9);
        foreach (Match match in Regex.Matches(imgs, @"""(.*?)"","))
        {
          pages.Add(new MangaPage(chapter.Uri, new Uri(match.Groups[1].Value), i++));
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }

      chapter.Pages.AddRange(pages);
    }
  }
}
