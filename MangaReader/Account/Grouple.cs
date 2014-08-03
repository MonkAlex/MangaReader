using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;

namespace MangaReader.Account
{
    class Grouple
    {
        public static string MainUrl = @"http://grouple.ru/";

        /// <summary>
        /// Авторизирован ли на сайте.
        /// </summary>
        public static bool IsLogined { get; set; }

        /// <summary>
        /// Закладки.
        /// </summary>
        public static List<Manga> Bookmarks
        {
            get { return _bookmarsk ?? LoadBookmarks(); }
        }

        private static List<Manga> _bookmarsk; 

        /// <summary>
        /// Указатель блокировки клиента файла.
        /// </summary>
        private static readonly object ClientLock = new object();

        /// <summary>
        /// Клиент с куками авторизованного пользователя.
        /// </summary>
        private static readonly CookieClient Client = new CookieClient() { BaseAddress = MainUrl, Encoding = Encoding.UTF8 };

        /// <summary>
        /// Циклическая попытка залогниться.
        /// </summary>
        public static void LoginWhile()
        {
            var inOtherThread = new Thread(() =>
            {
                Login();
                while (!IsLogined)
                {
                    Thread.Sleep(1000);
                    Login();
                }
                LoadBookmarks();
            });
            inOtherThread.Start();
        }

        /// <summary>
        /// Авторизоваться на сайте.
        /// </summary>
        public static void Login()
        {
            if (IsLogined)
                return;

            var login = Settings.Login;
            if (login == null)
                return;

            var loginData = new NameValueCollection
            {
                {"j_username", login.Name},
                {"j_password", login.Password},
                {"remember_me", "checked"}
            };
            lock (ClientLock)
            {
                try
                {
                    Client.UploadValues("internal/auth/j_spring_security_check", "POST", loginData);
                    IsLogined = Page.GetPage(MainUrl, Client).Contains("internal/auth/logout");
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }
        }

        /// <summary>
        /// Выйти с сайта.
        /// </summary>
        public static void Logout()
        {
            IsLogined = false;
            _bookmarsk = null;
            lock (ClientLock)
            {
                Page.GetPage(MainUrl + "internal/auth/logout", Client);
            }
        }

        /// <summary>
        /// Загрузить закладки.
        /// </summary>
        /// <returns></returns>
        public static List<Manga> LoadBookmarks()
        {
            var bookmarks = new List<Manga>();
            var document = new HtmlDocument();
            lock (ClientLock)
            {
                document.LoadHtml(Page.GetPage(@"http://grouple.ru/private/bookmarks", Client));
            }

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
                .Select(s => new Manga() { Url = s, Name = Getter.GetMangaName(Page.GetPage(s)).ToString() })
                .ToList();
            _bookmarsk = bookmarks;
            return bookmarks;
        }
    }
}
