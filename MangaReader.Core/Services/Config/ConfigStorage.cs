using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using MangaReader.Core.NHibernate;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MangaReader.Core.Services.Config
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
          if (Mapping.Initialized)
            databaseConfig = Repository.Get<DatabaseConfig>().SingleOrCreate();
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
    /// Подключенные плагины.
    /// </summary>
    internal static IEnumerable<IPlugin> Plugins { get { return ImportPlugins(); } }

    /// <summary>
    /// Настройки программы.
    /// </summary>
    private static string SettingsPath { get { return Path.Combine(WorkFolder, "settings.json"); } }

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
      catch (System.Exception e)
      {
        Log.Exception(e, "Fail load settings.");
      }

      if (storage == null)
      {
        storage = new ConfigStorage();
        Log.Add("Settings not found, create new default settings file.");
      }

      _instance = storage;
    }

    private static IEnumerable<IPlugin> ImportPlugins()
    {
      var catalog = new DirectoryCatalog(Path.Combine(ConfigStorage.WorkFolder, "lib"));
      var container = new CompositionContainer(catalog);

      try
      {
        return container.GetExportedValues<IPlugin>();
      }
      catch (ChangeRejectedException ex)
      {
        Log.Exception(ex, "Error on plugin loading.");
      }
      return Enumerable.Empty<IPlugin>();
    }

    public void Save()
    {
      if (Mapping.Initialized)
      {
        this.DatabaseConfig.Save();
        this.DatabaseConfig.MangaSettings.SaveAll();
      }

      var str = JsonConvert.SerializeObject(this);
      File.WriteAllText(SettingsPath, str, Encoding.UTF8);
      Log.Add("Settings saved.");
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