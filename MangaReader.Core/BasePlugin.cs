using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core
{
  /// <summary>
  /// Базовая реализация плагина для снижения дублирования кода.
  /// </summary>
  /// <remarks>Не обязательна к использованию.</remarks>
  public abstract class BasePlugin<T> : IPlugin where T : class, IPlugin, new()
  {
    public virtual string Name { get { return this.MangaType.Name; } }
    public abstract string ShortName { get; }
    public abstract Assembly Assembly { get; }
    public abstract Guid MangaGuid { get; }
    public abstract Type MangaType { get; }
    public abstract Type LoginType { get; }

    public static T Instance { get { return ConfigStorage.Plugins.OfType<T>().Single(); } }

    protected CookieContainer CookieContainer = new CookieContainer();

    public async Task<ISiteHttpClient> GetCookieClient(bool withLogin)
    {
      var login = NHibernate.Repository.GetStateless<MangaSetting>().Where(m => m.Manga == this.MangaGuid).Select(m => m.Login).Single();
      var mainUri = login.MainUri;
      var client = SiteHttpClientFactory.Get(mainUri, MangaSettingCache.Get(typeof(T)).Proxy, CookieContainer);
      this.ConfigureCookieClient(client, login);
      if (withLogin && !login.IsLogined(MangaGuid))
        await login.DoLogin(MangaGuid).ConfigureAwait(false);
      return client;
    }

    protected virtual void ConfigureCookieClient(ISiteHttpClient client, ILogin login)
    {

    }

    public virtual MangaSetting GetSettings()
    {
#warning will be reworked -- entities returned with closed session
      using (var context = NHibernate.Repository.GetEntityContext("Just get settings"))
        return context.Get<MangaSetting>().Single(m => m.Manga == this.MangaGuid);
    }

    public abstract ISiteParser GetParser();
    public abstract HistoryType HistoryType { get; }
  }
}
