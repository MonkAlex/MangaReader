using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MangaReader.Mangas;

namespace MangaReader
{
    public static class Getter
    {
        /// <summary>
        /// Получить название манги.
        /// </summary>
        /// <param name="mangaMainPage">Содержимое страницы манги.</param>
        /// <returns>Мультиязыковый класс с именем манги.</returns>
        public static MangaName GetMangaName(string mangaMainPage)
        {
            var name = new MangaName();
            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(mangaMainPage);
                var japNode = document.DocumentNode.SelectSingleNode("//div[@class=\"leftContent manga-page\"]//span[@class='jp-name']");
                if (japNode != null)
                    name.Japanese = japNode.InnerText;
                var engNode = document.DocumentNode.SelectSingleNode("//div[@class=\"leftContent manga-page\"]//span[@class='eng-name']");
                if (engNode != null)
                    name.English = engNode.InnerText;
                var rusNode = document.DocumentNode.SelectSingleNode("//div[@class=\"leftContent manga-page\"]//span[@class='name']");
                if (rusNode != null)
                    name.Russian = rusNode.InnerText;
            }
            catch (NullReferenceException ex) { Log.Add(ex.Message, Log.Level.None); }
            return name;
        }

        /// <summary>
        /// Получить ссылки на главы манги.
        /// </summary>
        /// <param name="mangaMainPage">Содержимое страницы манги.</param>
        /// <param name="url">Ссылка на мангу.</param>
        /// <returns>Словарь (ссылка, описание).</returns>
        public static Dictionary<string, string> GetLinksOfMangaChapters(string mangaMainPage, string url)
        {
            var dic = new Dictionary<string, string>();
            var links = new List<string> { };
            var description = new List<string> { };
            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(mangaMainPage);
                links = document.DocumentNode
                    .SelectNodes("//div[@class=\"expandable chapters-link\"]//td[@class=\" \"]//a[@href]")
                    .ToList()
                    .ConvertAll(r => r.Attributes.ToList().ConvertAll(i => i.Value))
                    .SelectMany(j => j)
                    .Where(k => k != string.Empty)
                    .Select(s => @"http://" + new Uri(url).Host + s + "?mature=1")
                    .Reverse()
                    .ToList();
                description = document.DocumentNode
                    .SelectNodes("//div[@class=\"expandable chapters-link\"]//tr//td[@class=\" \"]")
                    .Reverse()
                    .ToList()
                    .ConvertAll(r => r.InnerText.Replace("\r\n", string.Empty).Trim())
                    .ToList();
            }
            catch (NullReferenceException ex) { Log.Add(ex.Message + " - ошибка получения списка глав.", Log.Level.Error); }
            catch (ArgumentNullException ex) { Log.Add(ex.Message + " - главы не найдены.", Log.Level.Warning); }

            for (var i = 0; i < links.Count; i++)
            {
                dic.Add(links[i], description[i]);
            }

            return dic;
        }

        /// <summary>
        /// Получить ссылки на все изображения в главе.
        /// </summary>
        /// <param name="url">Ссылка на главу.</param>
        /// <returns>Список ссылок на изображения главы.</returns>
        public static List<string> GetImagesLink(string url)
        {
            var chapterLinksList = new List<string>();
            var document = new HtmlDocument();
            document.LoadHtml(Page.GetPage(url));

            var firstOrDefault = document.DocumentNode
                .SelectNodes("//div[@class=\"pageBlock reader-bottom\"]//script[@type=\"text/javascript\"]")
                .FirstOrDefault();

            if (firstOrDefault == null) 
                return chapterLinksList;

            var theNode = firstOrDefault.OuterHtml;
            chapterLinksList = Regex
                .Matches(theNode, @"{url:.[/:a-z0-9\-\._()\[\]&+]+", RegexOptions.IgnoreCase)
                .OfType<Match>()
                .Select(m => m.Groups[0].Value.Remove(0, 6))
                .ToList();
            return chapterLinksList;
        }
    }
}
