using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

    public MangaSettingCache(MangaSetting setting)
    {
      this.Plugin = ConfigStorage.Plugins.Single(p => p.MangaGuid == setting.Manga).GetType();
      this.Proxy = setting.ProxySetting.GetProxy();
    }
  }
}
