using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using MangaReader.Account;
using MangaReader.Services;

namespace MangaReader.Manga.Acomic
{
  public static class Getter
  {

    public static CookieClient GetAdultClient(string url)
    {
      var client = new CookieClient() { Encoding = Encoding.UTF8 };
      var loginData = new NameValueCollection
            {
                {"ageRestrict", "40"}
            };
      var result = client.UploadValues(url, "POST", loginData);
      return client;
    }

    /// <summary>
    /// Получить название манги.
    /// </summary>
    /// <param name="url">Ссылка на мангу.</param>
    /// <returns>Мультиязыковый класс с именем манги.</returns>
    public static string GetMangaName(string url)
    {
      var name = string.Empty;
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(Page.GetPage(url + @"/about", Getter.GetAdultClient(url)));
        var nameNode = document.DocumentNode.SelectSingleNode("//header[@class=\"serial\"]//img");
        if (nameNode != null && nameNode.Attributes.Count > 1)
          name = nameNode.Attributes[1].Value;
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
      return WebUtility.HtmlDecode(name);
    }

    /// <summary>
    /// Получить главы манги.
    /// </summary>
    /// <param name="url">Ссылка на мангу.</param>
    /// <returns>Словарь (ссылка, описание).</returns>
    public static List<Chapter> GetMangaChapters(string url)
    {
      var links = new List<string> { };
      var description = new List<string> { };
      var images = new List<string> { };
      try
      {
        var adultClient = Getter.GetAdultClient(url);
        var document = new HtmlDocument();
        document.LoadHtml(Page.GetPage(url, adultClient));
        var last = document.DocumentNode.SelectSingleNode("//nav[@class='serial']//a[@class='read2']").Attributes[1].Value;
        var count = int.Parse(last.Remove(0, last.LastIndexOf('/') + 1));
        var list = @"http://" + new Uri(url).Host + document.DocumentNode.SelectSingleNode("//nav[@class='serial']//a[@class='read3']").Attributes[1].Value;
        for (var i = 0; i < count; i = i + 5)
        {
          var currentList = list + "?skip=" + i;
          document.LoadHtml(Page.GetPage(currentList, adultClient));
          links.AddRange(document.DocumentNode
              .SelectNodes("//div[@class=\"issue\"]//a")
              .Select(r => r.Attributes[0].Value)
              .ToList());
          description.AddRange(document.DocumentNode
              .SelectNodes("//div[@class=\"issue\"]//img")
              .Select(r => r.Attributes[1].Value)
              .ToList());
          images.AddRange(document.DocumentNode
              .SelectNodes("//div[@class=\"issue\"]//img")
              .Select(r => @"http://" + new Uri(url).Host + r.Attributes[0].Value)
              .ToList());
        }
      }
      catch (NullReferenceException ex) { Log.Exception(ex, "Ошибка получения списка глав.", url); }
      catch (ArgumentNullException ex) { Log.Exception(ex, "Главы не найдены.", url); }

      return links.Select((t, i) => new Chapter(t, description[i], images[i])).ToList();
    }
  }
}
