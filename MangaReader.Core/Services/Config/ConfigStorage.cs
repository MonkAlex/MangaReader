using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using MangaReader.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NHibernate.Linq;

namespace MangaReader.Services.Config
{
  public class ConfigStorage
  {
    public static ConfigStorage Instance
    {
      get
      {
        if (_instance == null)
          Load();
        return _instance;
      }
    }

    private static ConfigStorage _instance;

    /// <summary>
    /// Настройки программы.
    /// </summary>
    public AppConfig AppConfig { get; set; }

    /// <summary>
    /// Настройки внешнего вида.
    /// </summary>
    public ViewConfig ViewConfig { get; set; }

    /// <summary>
    /// Настройки, зависящие от базы данных.
    /// </summary>
    [JsonIgnore]
    public DatabaseConfig DatabaseConfig
    {
      get
      {
        if (databaseConfig == null)
        {
          if (Mapping.Environment.Initialized)
            databaseConfig = Mapping.Repository.Get<DatabaseConfig>().SingleOrCreate();
          else
            Log.Exception("Запрос DatabaseConfig до инициализации соединения с базой.");
        }
        return databaseConfig;
      }
      set { }
    }

    private DatabaseConfig databaseConfig;

    /// <summary>
    /// Папка программы.
    /// </summary>
    internal static string WorkFolder { get { return workFolder; } }

    private static string workFolder = AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// Настройки программы.
    /// </summary>
    private static string SettingsPath { get { return Path.Combine(WorkFolder, "settings.json"); } }

    [Obsolete("Только для конвертации.")]
    internal static readonly string SettingsOldPath = WorkFolder + "\\settings.xml";

    public static void Load()
    {
      if (_instance != null)
        return;

      JsonConvert.DefaultSettings = () =>
      new JsonSerializerSettings
      {
        Formatting = Formatting.Indented,
        Converters = new List<JsonConverter> { new StringEnumConverter(), new VersionConverter() }
      };

      ConfigStorage storage = null;
      try
      {
        if (File.Exists(SettingsPath))
        {
          storage = JsonConvert.DeserializeObject<ConfigStorage>(File.ReadAllText(SettingsPath, Encoding.UTF8));
        }
      }
      catch (Exception e)
      {
        Log.Exception(e, "Fail load settings.");
      }

      if (storage == null)
        storage = new ConfigStorage();

      _instance = storage;
    }

    public void Save()
    {
      this.DatabaseConfig.Save();
      this.DatabaseConfig.MangaSettings.SaveAll();
      var str = JsonConvert.SerializeObject(this);
      File.WriteAllText(SettingsPath, str, Encoding.UTF8);
    }

    public static void Close()
    {
      Instance.databaseConfig = null;
      _instance = null;
    }

    public ConfigStorage()
    {
      this.AppConfig = new AppConfig();
      this.ViewConfig = new ViewConfig();
    }
  }
}