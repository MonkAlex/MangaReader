using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MangaReader.Manga;
using MangaReader.Services;

namespace MangaReader.Account
{
  public abstract class Login : Entity.Entity
  {
    public static Guid Type { get { return Guid.Empty; } }

    public static Guid Manga { get { return Guid.Empty; } }

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

    public static Login Create(Guid manga)
    {
      var baseClass = typeof(Login);
      // TODO: с учетом подключаемых либ - искать надо везде.
      var types = baseClass.Assembly.GetTypes()
        .Where(type => type.IsSubclassOf(baseClass));

      Login login = null;
      foreach (var type in types)
      {
        if (Equals(type.MangaProperty(), manga))
          login = (Login)Activator.CreateInstance(type);
      }
      return login;
    }

    protected Login()
    {
      this.ClientLock = new object();
    }
  }
}
