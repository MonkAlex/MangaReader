using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MangaReader.Core.Account;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{
  public class MangaSettingCache
  {
    public Type Plugin { get; set; }

    public IWebProxy Proxy { get; set; }

    public int ProxySettingId { get; set; }

    public bool IsParent { get; private set; }

    public ProxySettingType SettingType { get; set; }

    private void RefreshProperties(ProxySetting setting)
    {
      this.ProxySettingId = setting.Id;
      this.Proxy = setting.GetProxy();
      this.SettingType = setting.SettingType;
    }

    private static ConcurrentDictionary<Type, MangaSettingCache> caches = new ConcurrentDictionary<Type, MangaSettingCache>();

    public static readonly Type RootPluginType = typeof(IPlugin);

    public static Type GetPluginType(MangaSetting mangaSetting)
    {
      return ConfigStorage.Plugins.Single(p => p.MangaGuid == mangaSetting.Manga).GetType();
    }

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

    internal static void RevalidateSetting(ProxySetting setting)
    {
      RevalidateSetting(null, setting);
    }

    internal static void RevalidateSetting(Type pluginType, ProxySetting setting)
    {
      if (caches.Count == 0)
        return;

      var inCache = pluginType == null ?
        caches.Values.Where(c => c.ProxySettingId == setting.Id).ToList() :
        caches.Values.Where(c => c.Plugin == pluginType).ToList();

      foreach (var cache in inCache.OrderByDescending(c => c.IsParent))
        cache.RefreshProperties(setting);

      Log.Add($"Applied {setting.SettingType} proxy to {string.Join(", ", inCache.Select(s => s.Plugin.Name))}");

      // If any of setting cache is root - need recreate all 'child' caches.
      if (inCache.Any(r => r.IsParent))
      {
        var childs = caches.Values.Where(c => c.SettingType == ProxySettingType.Parent).ToList();
        foreach (var cache in childs)
          cache.Proxy = setting.GetProxy();

        Log.Add($"Applied {setting.SettingType} proxy to childs {string.Join(", ", childs.Select(s => s.Plugin.Name))}");
      }
    }

    public MangaSettingCache(MangaSetting mangaSetting) : this(GetPluginType(mangaSetting), mangaSetting.ProxySetting)
    {
      
    }

    public MangaSettingCache(DatabaseConfig config) : this(RootPluginType, config.ProxySetting)
    {
      IsParent = true;
    }

    public MangaSettingCache(Type pluginType, ProxySetting setting)
    {
      this.Plugin = pluginType;
      this.RefreshProperties(setting);
      this.IsParent = false;
    }
  }
}
