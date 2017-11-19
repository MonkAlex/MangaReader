using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <param name="query">Коллекция уже имеющихся настроек.</param>
    /// <returns>Коллекция всех настроек.</returns>
    private static void CreateDefaultMangaSettings(List<MangaSetting> query)
    {
      var settings = new List<MangaSetting>(query);
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
          Login = Login.Get(plugin.LoginType)
        };

        setting.Save();
        settings.Add(setting);
      }
    }

    public DatabaseConfig()
    {
      this.Version = new Version(1, 0, 0);
    }

    public static void Initialize()
    {
      Repository.GetStateless<DatabaseConfig>().SingleOrCreate();
      var query = Repository.GetStateless<MangaSetting>().ToList();
      CreateDefaultMangaSettings(query);
    }
  }
}