using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Core.NHibernate;
using NHibernate.Linq;

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
          Login = await Login.Get(plugin.LoginType).ConfigureAwait(false)
        };

        await context.Save(setting).ConfigureAwait(false);
        settings.Add(setting);
      }
    }

    public DatabaseConfig()
    {
      this.Version = new Version(1, 0, 0);
      this.FolderNamingStrategy = Generic.GetNamingStrategyId<NumberPrefixFolderNaming>();
    }

    public static async Task Initialize()
    {
      await Repository.GetStateless<DatabaseConfig>().SingleOrCreate().ConfigureAwait(false);
      using (var context = Repository.GetEntityContext("Initialize database config"))
        await CreateDefaultMangaSettings(context).ConfigureAwait(false);
    }
  }
}