using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Linq;

namespace MangaReader.Services.Config
{
  public class DatabaseConfig
  {
    /// <summary>
    /// Версия базы данных.
    /// </summary>
    public Version Version { get; set; }

    /// <summary>
    /// Настройки разных типов манги.
    /// </summary>
    internal List<MangaSetting> MangaSettings
    {
      get
      {
        if (mangaSettings == null)
        {
          if (Mapping.Environment.Initialized)
          {
            var query = Mapping.Environment.Session.Query<MangaSetting>().ToList();
            mangaSettings = CreateDefaultMangaSettings(query);
            ConfigStorage.Instance.Save();
          }
          else
          {
            Log.Exception("Запрос MangaSettings до инициализации соединения с базой.");
            return new List<MangaSetting>();
          }
        }
        return mangaSettings;
      }
      set { }
    }

    private static List<MangaSetting> mangaSettings;

    /// <summary>
    /// Создать дефолтные настройки для новых типов.
    /// </summary>
    /// <param name="query">Коллекция уже имеющихся настроек.</param>
    /// <returns>Коллекция всех настроек.</returns>
    private static List<MangaSetting> CreateDefaultMangaSettings(List<MangaSetting> query)
    {
      var baseClass = typeof(Manga.Mangas);
      var types = Assembly.GetAssembly(baseClass).GetTypes()
        .Where(type => type.IsSubclassOf(baseClass));

      foreach (var type in types)
      {
        if (query.Any(s => Equals(s.Manga, type.TypeProperty())))
          continue;

        var setting = new MangaSetting
        {
          Folder = AppConfig.DownloadFolder,
          Manga = type.TypeProperty(),
          MangaName = type.Name,
          DefaultCompression = Compression.CompressionMode.Manga
        };

        query.Add(setting);
      }
      return query;
    }

    public DatabaseConfig()
    {
      this.Version = new Version(1, 0, 0);
    }
  }
}