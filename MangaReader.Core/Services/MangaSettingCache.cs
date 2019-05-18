using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{
  public class MangaSettingCache
  {
    public Type Plugin { get; set; }

    public System.Net.IWebProxy Proxy { get; set; }

    private static ConcurrentDictionary<Type, MangaSettingCache> caches = new ConcurrentDictionary<Type, MangaSettingCache>();

    public static void Set(MangaSettingCache cache)
    {
      caches.AddOrUpdate(cache.Plugin, cache, (_, __) => cache);
    }

    public static MangaSettingCache Get(Type pluginType)
    {
      MangaSettingCache cache;
      if (caches.TryGetValue(pluginType, out cache))
        return cache;
      throw new KeyNotFoundException($"В кеше не найдено ничего с типом {pluginType.Name}");
    }

    public static void Clear()
    {
      caches.Clear();
    }

    public static async Task RevalidateCache()
    {
      Clear();

      using (var context = Repository.GetEntityContext())
      {
        var config = await context.Get<DatabaseConfig>().SingleAsync().ConfigureAwait(false);
        var parentSetting = config.ProxySetting;
        if (parentSetting != null)
        {
          MangaSettingCache.Set(new MangaSettingCache()
          {
            Plugin = typeof(IPlugin),
            Proxy = parentSetting.GetProxy()
          });
        }

        var settings = await context.Get<MangaSetting>().Where(s => s.ProxySetting != null).ToListAsync().ConfigureAwait(false);
        foreach (var setting in settings)
        {
          var plugin = ConfigStorage.Plugins.Single(p => p.MangaGuid == setting.Manga).GetType();
          MangaSettingCache.Set(new MangaSettingCache
          {
            Plugin = plugin,
            Proxy = setting.ProxySetting.SettingType == ProxySettingType.Parent ? Get(typeof(IPlugin)).Proxy : setting.ProxySetting.GetProxy()
          });
        }

        foreach (var grouped in settings.GroupBy(s => s.ProxySetting.SettingType))
          Log.Add($"Applied {grouped.Key} proxy to {string.Join(", ", grouped.Select(s => s.MangaName))}");
      }
    }
  }
}
