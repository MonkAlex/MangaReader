using System;
using System.Collections.Generic;
using MangaReader.Manga;
using MangaReader.Services;

namespace MangaReader.Account
{
  public class Login : Entity.Entity
  {
    public static Guid Type { get { return Guid.Parse("EC4D4CDE-EF54-4B67-AF48-1B7909709D5C"); } }

    public virtual bool IsLogined { get; set; }

    public virtual string Name { get; set; }

    public virtual string Password { get; set; }

    public virtual bool CanLogin { get { return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Password); } }

    public virtual Uri MainUri { get; set; }

    public virtual Uri LogoutUri { get; set; }

    public virtual Uri BookmarksUri { get; set; }

    /// <summary>
    /// Указатель блокировки клиента файла.
    /// </summary>
    protected internal virtual object ClientLock { get; set; }

    /// <summary>
    /// Клиент с куками авторизованного пользователя.
    /// </summary>
    protected internal virtual CookieClient Client
    {
      get { return this.client ?? (this.client = new CookieClient() {BaseAddress = MainUri.ToString()}); }
    }

    private CookieClient client;
    
    public virtual void DoLogin()
    {

    }

    public virtual void Logout()
    {
      IsLogined = false;
      using (TimedLock.Lock(ClientLock))
      {
        Page.GetPage(LogoutUri, Client);
      }
    }

    public virtual List<Mangas> GetBookmarks()
    {
      return new List<Mangas>();
    }

    public Login()
    {
      this.ClientLock = new object();
    }
  }
}
