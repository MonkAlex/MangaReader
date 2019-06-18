using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Exception;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using NUnit.Framework;

namespace Tests.Entities.MangaSetting
{
  public class ProxyTests : TestClass
  {
    [Test]
    public async Task CreateDelete()
    {
      using (var context = Repository.GetEntityContext())
      {
        var proxy = new ProxySetting(ProxySettingType.Manual);
        await context.Save(proxy).ConfigureAwait(false);
        var proxyId = proxy.Id;
        proxy = await context.Get<ProxySetting>().SingleAsync(s => s.Id == proxyId).ConfigureAwait(false);
        Assert.NotNull(proxy);

        await context.Delete(proxy).ConfigureAwait(false);
        proxy = await context.Get<ProxySetting>().SingleOrDefaultAsync(s => s.Id == proxyId).ConfigureAwait(false);
        Assert.Null(proxy);
      }
    }

    /// <summary>
    /// Delete must be failed, when entity used in other.
    /// </summary>
    [Test]
    public async Task CreateUseAndBlockDelete()
    {
      var settingsId = 0;
      var proxyId = 0;
      using (var context = Repository.GetEntityContext())
      {
        var proxy = new ProxySetting(ProxySettingType.Manual);
        await context.Save(proxy).ConfigureAwait(false);

        var setting = new MangaReader.Core.Services.MangaSetting() { Folder = AppConfig.DownloadFolder };
        setting.ProxySetting = proxy;
        await context.Save(setting).ConfigureAwait(false);

        async Task DeleteProxy()
        {
          await context.Delete(proxy).ConfigureAwait(false);
        }

        settingsId = setting.Id;
        proxyId = proxy.Id;
        Assert.ThrowsAsync<EntityException<ProxySetting>>(DeleteProxy);
      }

      using (var context = Repository.GetEntityContext())
      {
        var setting = await context.Get<MangaReader.Core.Services.MangaSetting>().SingleAsync(s => s.Id == settingsId).ConfigureAwait(false);
        var proxy = await context.Get<ProxySetting>().SingleAsync(s => s.Id == proxyId).ConfigureAwait(false);
        await context.Delete(setting).ConfigureAwait(false);
        await context.Delete(proxy).ConfigureAwait(false);
      }
    }

    /// <summary>
    /// When username-password changed - only proxy property of cache must be changed.
    /// </summary>
    [Test]
    public async Task ChangeProxySetting()
    {
      using (var context = Repository.GetEntityContext())
      {
        var proxySetting = new ProxySetting(ProxySettingType.Manual);
        await context.Save(proxySetting).ConfigureAwait(false);
        var pseudoPluginType = typeof(ProxyTests);
        MangaSettingCache.Set(new MangaSettingCache(pseudoPluginType, proxySetting));

        var cache = MangaSettingCache.Get(pseudoPluginType);
        var proxy = cache.Proxy;

        void AssertCacheEquals()
        {
          Assert.AreEqual(proxySetting.Id, cache.ProxySettingId);
          Assert.AreEqual(pseudoPluginType, cache.Plugin);
          Assert.AreEqual(ProxySettingType.Manual, cache.SettingType);
        }

        AssertCacheEquals();
        Assert.AreEqual(proxy, cache.Proxy);

        proxySetting.UserName = "123";
        await context.Save(proxySetting).ConfigureAwait(false);

        AssertCacheEquals();
        Assert.AreNotEqual(proxy, cache.Proxy);

        await context.Delete(proxySetting).ConfigureAwait(false);
      }
    }
    
    /// <summary>
    /// Use 'parent' proxy, change app proxy A->B->A and validate caches.
    /// </summary>
    [Test]
    public async Task ChangeDatabaseConfig([Values]bool withParentSettingChange)
    {
      using (var context = Repository.GetEntityContext())
      {
        var proxySetting = new ProxySetting(ProxySettingType.Parent);
        await context.Save(proxySetting).ConfigureAwait(false);
        var pseudoPluginType = typeof(Repository);
        MangaSettingCache.Set(new MangaSettingCache(pseudoPluginType, proxySetting));

        var settingCache = MangaSettingCache.Get(pseudoPluginType);
        var settingCacheProxy = settingCache.Proxy;

        void AssertCacheEquals()
        {
          Assert.AreEqual(proxySetting.Id, settingCache.ProxySettingId);
          Assert.AreEqual(pseudoPluginType, settingCache.Plugin);
          Assert.AreEqual(ProxySettingType.Parent, settingCache.SettingType);
        }

        AssertCacheEquals();
        Assert.AreEqual(settingCacheProxy, settingCache.Proxy);

        ProxySetting setting = null;
        if (withParentSettingChange)
        {
          var databaseConfig = await context.Get<DatabaseConfig>().SingleAsync().ConfigureAwait(false);
          setting = databaseConfig.ProxySetting;
          databaseConfig.ProxySetting = await context.Get<ProxySetting>().SingleAsync(p => p.SettingType == ProxySettingType.NoProxy).ConfigureAwait(false);
          await context.Save(databaseConfig).ConfigureAwait(false);
        }

        var parentCache = MangaSettingCache.Get(MangaSettingCache.RootPluginType);

        Assert.AreNotEqual(settingCache, parentCache);

        if (withParentSettingChange)
        {
          Assert.AreNotEqual(settingCacheProxy, parentCache.Proxy);
          Assert.AreEqual(settingCache.Proxy, parentCache.Proxy);
          Assert.AreNotEqual(settingCacheProxy, settingCache.Proxy);
          AssertCacheEquals();
        }
        else
        {
          Assert.AreEqual(settingCacheProxy, parentCache.Proxy);
          Assert.AreEqual(settingCache.Proxy, parentCache.Proxy);
        }

        if (withParentSettingChange)
        {
          var databaseConfig = await context.Get<DatabaseConfig>().SingleAsync().ConfigureAwait(false);
          databaseConfig.ProxySetting = setting;
          await context.Save(databaseConfig).ConfigureAwait(false);

          AssertCacheEquals();
          Assert.AreEqual(settingCache.Proxy, parentCache.Proxy);
        }

        await context.Delete(proxySetting).ConfigureAwait(false);
      }
    }

    [Test]
    public async Task ChangeMangaSetting()
    {
      using (var context = Repository.GetEntityContext())
      {
        var proxySetting = new ProxySetting(ProxySettingType.Manual);
        await context.Save(proxySetting).ConfigureAwait(false);
        var mangaSetting = await context.Get<MangaReader.Core.Services.MangaSetting>()
          .FirstOrDefaultAsync(s => s.ProxySetting.SettingType == ProxySettingType.Parent)
          .ConfigureAwait(false);

        var pluginType = MangaSettingCache.GetPluginType(mangaSetting);
        var cache = MangaSettingCache.Get(pluginType);
        var proxy = cache.Proxy;

        Assert.AreEqual(pluginType, cache.Plugin);
        Assert.AreNotEqual(proxySetting.Id, cache.ProxySettingId);
        Assert.AreNotEqual(ProxySettingType.Manual, cache.SettingType);

        Assert.AreEqual(proxy, cache.Proxy);

        var originalProxy = mangaSetting.ProxySetting;
        mangaSetting.ProxySetting = proxySetting;
        await context.Save(mangaSetting).ConfigureAwait(false);

        Assert.AreEqual(pluginType, cache.Plugin);
        Assert.AreEqual(proxySetting.Id, cache.ProxySettingId);
        Assert.AreEqual(ProxySettingType.Manual, cache.SettingType);

        Assert.AreNotEqual(proxy, cache.Proxy);

        mangaSetting.ProxySetting = originalProxy;
        await context.Save(mangaSetting).ConfigureAwait(false);

        Assert.AreEqual(pluginType, cache.Plugin);
        await context.Delete(proxySetting).ConfigureAwait(false);
      }
    }
  }
}
