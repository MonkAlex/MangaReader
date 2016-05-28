using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Настройки разных типов манги.
    /// </summary>
    public List<MangaSetting> MangaSettings
    {
      get
      {
        if (mangaSettings == null)
        {
          if (Mapping.Initialized)
          {
            var query = Repository.Get<MangaSetting>().ToList();
            mangaSettings = CreateDefaultMangaSettings(query);

            if (mangaSettings.Except(query).Any())
              this.mangaSettings.SaveAll();
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

    private List<MangaSetting> mangaSettings;

    /// <summary>
    /// Создать дефолтные настройки для новых типов.
    /// </summary>
    /// <param name="query">Коллекция уже имеющихся настроек.</param>
    /// <returns>Коллекция всех настроек.</returns>
    private static List<MangaSetting> CreateDefaultMangaSettings(List<MangaSetting> query)
    {
      var types = Generic.GetAllTypes<Manga.Mangas>();
      var settings = new List<MangaSetting>(query);

      foreach (var type in types)
      {
        var typeProperty = type.TypeProperty();
        if (settings.Any(s => Equals(s.Manga, typeProperty)))
          continue;

        var setting = new MangaSetting
        {
          Folder = AppConfig.DownloadFolder,
          Manga = typeProperty,
          MangaName = type.Name,
          DefaultCompression = Compression.CompressionMode.Manga
        };

        settings.Add(setting);
      }
      return settings;
    }

    public DatabaseConfig()
    {
      this.Version = new Version(1, 0, 0);
    }
  }
}