using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Core.NHibernate;

namespace MangaReader.Core.Services.Config
{
  public class DatabaseConfig : Entity.Entity
  {
    /// <summary>
    /// Версия базы данных.
    /// </summary>
    public Version Version { get; set; }

    /// <summary>
    /// Идентификатор выбранной стратегии именования папок.
    /// </summary>
    public Guid FolderNamingStrategy { get; set; }

    /// <summary>
    /// Уникальный идентификатор базы данных.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Guid UniqueId
    {
      get
      {
        if (uniqueId == Guid.Empty)
          uniqueId = Guid.NewGuid();
        return uniqueId;
      }
      set
      {
        if (uniqueId == Guid.Empty)
          uniqueId = value;
      }
    }

    private Guid uniqueId;

    /// <summary>
    /// Настройки прокси.
    /// </summary>
    public ProxySetting ProxySetting { get; set; }

    /// <summary>
    /// Создать дефолтные настройки для новых типов.
    /// </summary>
    /// <param name="context">Контекст подключения к БД.</param>
    /// <returns>Коллекция всех настроек.</returns>
    private static async Task CreateDefaultMangaSettings(RepositoryContext context)
    {
      var settings = await context.Get<MangaSetting>().ToListAsync().ConfigureAwait(false);
      var plugins = ConfigStorage.Plugins;
      foreach (var plugin in plugins)
      {
        if (settings.Any(s => Equals(s.Manga, plugin.MangaGuid)))
          continue;

        var setting = new MangaSetting
        {
          Folder = AppConfig.DownloadFolderName,
          Manga = plugin.MangaGuid,
          MangaName = plugin.Name,
          DefaultCompression = Compression.CompressionMode.Manga,
          ProxySetting = await context.Get<ProxySetting>().SingleAsync(s => s.SettingType == ProxySettingType.Parent).ConfigureAwait(false),
          Login = await Login.Get(plugin.LoginType).ConfigureAwait(false)
        };

        await context.Save(setting).ConfigureAwait(false);
        settings.Add(setting);
      }

      await MangaSettingCache.RevalidateCache().ConfigureAwait(false);
    }

    private static async Task<List<ProxySetting>> CreateDefaultProxySettings(RepositoryContext context)
    {
      var types = new[] { ProxySettingType.NoProxy, ProxySettingType.System, ProxySettingType.Parent };
      var created = new List<ProxySetting>();
      foreach (var settingType in types)
      {
        if (!await context.Get<ProxySetting>().AnyAsync(s => s.SettingType == settingType).ConfigureAwait(false))
        {
          var proxy = new ProxySetting(settingType);
          await context.Save(proxy).ConfigureAwait(false);
          created.Add(proxy);
        }
      }

      return created;
    }

    public DatabaseConfig()
    {
      this.Version = new Version(1, 0, 0);
      this.FolderNamingStrategy = Generic.GetNamingStrategyId<NumberPrefixFolderNaming>();
    }

    public static async Task Initialize()
    {
      using (var context = Repository.GetEntityContext("Initialize database config"))
      {
        var config = await Repository.GetStateless<DatabaseConfig>().SingleOrCreate().ConfigureAwait(false);
        var proxySettings = await CreateDefaultProxySettings(context).ConfigureAwait(false);
        if (config.ProxySetting == null)
        {
          config.ProxySetting = proxySettings.FirstOrDefault(s => s.SettingType == ProxySettingType.System);
          await context.Save(config).ConfigureAwait(false);
        }
        await CreateDefaultMangaSettings(context).ConfigureAwait(false);
      }
    }
  }
}
