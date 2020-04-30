using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Account
{
  public abstract class Login : Entity.Entity, ILogin
  {
    public bool IsLogined
    {
      get { return isLogined; }
      set
      {
        isLogined = value;
        OnLoginStateChanged(value);
      }
    }

    public event EventHandler<bool> LoginStateChanged;

    public virtual string Name { get; set; }

    public virtual string Password { get; set; }

    public virtual bool CanLogin { get { return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Password); } }

    public abstract Uri MainUri { get; set; }

    public abstract Uri LogoutUri { get; }

    public abstract Uri BookmarksUri { get; }

    private bool isLogined;

    public abstract Task<bool> DoLogin(Guid mangaType);

    public virtual async Task<bool> Logout(Guid mangaType)
    {
      IsLogined = false;
      var plugin = ConfigStorage.Plugins.Single(p => p.MangaGuid == mangaType);
      await Page.GetPageAsync(LogoutUri, plugin.GetCookieClient()).ConfigureAwait(false);
      return true;
    }

    public async Task<List<IManga>> GetBookmarks(Guid mangaType)
    {
      if (this.CanLogin)
      {
        Log.AddFormat("Start load bookmarks from '{0}'.", this.MainUri);
        var bookmarks = await DownloadBookmarks(mangaType).ConfigureAwait(false);
        Log.AddFormat("Finish load bookmarks from '{0}'.", this.MainUri);
        return bookmarks;
      }
      return new List<IManga>();
    }

    protected abstract Task<List<IManga>> DownloadBookmarks(Guid mangaType);

    public static async Task<ILogin> Get(Type type)
    {
      using (var context = NHibernate.Repository.GetEntityContext())
      {
        var logins = await context.Get<ILogin>().ToListAsync().ConfigureAwait(false);
        return logins.SingleOrDefault(l => l.GetType() == type) ?? (ILogin)Activator.CreateInstance(type);
      }
    }

    protected virtual void OnLoginStateChanged(bool e)
    {
      LoginStateChanged?.Invoke(this, e);
    }
  }
}
