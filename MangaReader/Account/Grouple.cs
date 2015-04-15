using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;
using MangaReader.Manga.Grouple;
using MangaReader.Services;

namespace MangaReader.Account
{
  class Grouple
  {
    public static Uri MainUri = new Uri(@"http://grouple.ru/");
    public static Uri LogoutUri = new Uri(MainUri + "internal/auth/logout");
    public static Uri BookmarksUri = new Uri(@"http://grouple.ru/private/bookmarks");

    /// <summary>
    /// Авторизирован ли на сайте.
    /// </summary>
    public static bool IsLogined { get; set; }

    public static Login SettingLogin = Settings.DownloadFolders.First(x => x.Manga == Readmanga.Type).Login;

    /// <summary>
    /// Закладки.
    /// </summary>
    public static List<Readmanga> Bookmarks
    {
      get { return _bookmarsk ?? LoadBookmarks(); }
    }

    private static List<Readmanga> _bookmarsk;

    /// <summary>
    /// Указатель блокировки клиента файла.
    /// </summary>
    private static readonly object ClientLock = new object();

    /// <summary>
    /// Клиент с куками авторизованного пользователя.
    /// </summary>
    private static readonly CookieClient Client = new CookieClient() { BaseAddress = MainUri.ToString(), Encoding = Encoding.UTF8 };

    /// <summary>
    /// Циклическая попытка залогниться.
    /// </summary>
    public static void LoginWhile()
    {
      var inOtherThread = new Thread(() =>
      {
        Login();
        while (!IsLogined && SettingLogin.CanLogin)
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


      if (SettingLogin == null || !SettingLogin.CanLogin)
        return;

      var loginData = new NameValueCollection
            {
                {"j_username", SettingLogin.Name},
                {"j_password", SettingLogin.Password},
                {"remember_me", "checked"}
            };
      using (TimedLock.Lock(ClientLock))
      {
        try
        {
          Client.UploadValues("internal/auth/j_spring_security_check", "POST", loginData);
          IsLogined = Page.GetPage(MainUri, Client).Contains("internal/auth/logout");
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
      using (TimedLock.Lock(ClientLock))
      {
        Page.GetPage(LogoutUri, Client);
      }
    }

    /// <summary>
    /// Загрузить закладки.
    /// </summary>
    /// <returns></returns>
    public static List<Readmanga> LoadBookmarks()
    {
      var bookmarks = new List<Readmanga>();
      var document = new HtmlDocument();
      using (TimedLock.Lock(ClientLock))
      {
        document.LoadHtml(Page.GetPage(BookmarksUri, Client));
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
          .Select(m => new Uri(m.Groups[1].Value))
          .Select(s => new Readmanga() { Uri = s, Name = Getter.GetMangaName(Page.GetPage(s)).ToString() })
          .ToList();
      _bookmarsk = bookmarks;
      return bookmarks;
    }
  }
}
