using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MangaReader.Logins
{
    class Grouple
    {
        public static string MainUrl = @"http://grouple.ru/";

        private static CookieClient client = new CookieClient() { BaseAddress = MainUrl, Encoding = Encoding.UTF8 };

        /// <summary>
        /// Авторизоваться на сайте.
        /// </summary>
        public static void Login()
        {
            var login = Settings.Login;
            if (login == null)
                return;

            var loginData = new NameValueCollection();
            loginData.Add("j_username", login.Name);
            loginData.Add("j_password", login.Password);
            loginData.Add("remember_me", "checked");
            client.UploadValues("internal/auth/j_spring_security_check", "POST", loginData);
        }

        /// <summary>
        /// Загрузить закладки.
        /// </summary>
        /// <returns></returns>
        public static List<Manga> LoadBookmarks()
        {
            var bookmarks = new List<Manga>();
            var document = new HtmlDocument();
            document.LoadHtml(Page.GetPage(@"http://grouple.ru/private/bookmarks", client));

            var firstOrDefault = document.DocumentNode
                .SelectNodes("//div[@class=\"bookmarks-lists\"]");

            if (firstOrDefault == null || firstOrDefault.FirstOrDefault() == null)
                return bookmarks;

            bookmarks = Regex
                .Matches(firstOrDefault.FirstOrDefault().OuterHtml, @"href='(.*?)'", RegexOptions.IgnoreCase)
                .OfType<Group>()
                .Select(g => g.Captures[0])
                .OfType<Match>()
                .Select(m => m.Groups[1].Value)
                .Select(s => new Manga(s))
                .ToList();
            return bookmarks;
        }
    }
}
