using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MangaReader.Core.Exception;
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
    /// Папка программы.
    /// </summary>
    public static string WorkFolder { get { return Loader.WorkFolder; } }

    /// <summary>
    /// Подключенные плагины.
    /// </summary>
    public static IList<IPlugin> Plugins { get { return plugins; } }

    private static IList<IPlugin> plugins;

    /// <summary>
    /// Настройки программы.
    /// </summary>
    private static string SettingsPath { get { return Path.Combine(WorkFolder, "settings.json"); } }

    /// <summary>
    /// Папка с либами программы.
    /// </summary>
    public static string LibPath { get { return Loader.LibPath; } }

    /// <summary>
    /// Папка с плагинами программы.
    /// </summary>
    public static string PluginPath { get { return Loader.PluginPath; } }

    /// <summary>
    /// Папки с плагинами.
    /// </summary>
    public static string[] PluginFolders { get; set; } = { PluginPath };

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

    public static IPlugin GetPlugin<T>() where T : Core.Manga.IManga
    {
      return Plugins.SingleOrDefault(p => p.MangaType == typeof(T));
    }

    public static void RefreshPlugins()
    {
      Loader.Init();
      var result = new List<IPlugin>();

      Log.Add("Search plugins...");
      foreach (var pluginFolder in PluginFolders)
        result.AddRange(GetPluginsFrom(pluginFolder));
      Log.Add("Search plugins completed.");

      plugins = result;
    }

    private static IEnumerable<IPlugin> GetPluginsFrom(string path)
    {
      if (Directory.Exists(path))
      {
        try
        {
          var result = new List<IPlugin>();
          var container = new CompositionContainer(new SafeDirectoryCatalog(path));
          var imanga = typeof(Manga.IManga);
          var ilogin = typeof(Account.ILogin);
          foreach (var plugin in container.GetExportedValues<IPlugin>())
          {
            try
            {
              if (!imanga.IsAssignableFrom(plugin.MangaType))
                throw new MangaReaderException($"Type in property {nameof(plugin.MangaType)} of " +
                                               $"type {plugin.GetType()} must be implement {imanga} interface.");

              if (!ilogin.IsAssignableFrom(plugin.LoginType))
                throw new MangaReaderException($"Type in property {nameof(plugin.LoginType)} of " +
                                               $"type {plugin.GetType()} must be implement {ilogin} interface.");

              Log.Add($"Plugin {plugin.Name}-{plugin.Assembly.GetName().Version} loaded from {DirectoryHelpers.GetRelativePath(WorkFolder, plugin.Assembly.Location)}.");
              result.Add(plugin);
            }
            catch (MangaReaderException mre)
            {
              Log.Exception(mre);
            }
          }

          return result;
        }
        catch (ReflectionTypeLoadException ex)
        {
          foreach (var exception in ex.LoaderExceptions)
            Log.Exception(exception, "Loader exception:");
        }
        catch (System.Exception ex)
        {
          Log.Exception(ex, string.Format("Plugins from {0} cannot be loaded.", path));
        }
      }
      return Enumerable.Empty<IPlugin>();
    }

    public void Save()
    {
      var str = JsonConvert.SerializeObject(this);
      File.WriteAllText(SettingsPath, str, Encoding.UTF8);
      Log.Add("Settings saved.");
    }

    public static void Close()
    {
      _instance = null;
    }

    public ConfigStorage()
    {
      this.AppConfig = new AppConfig();
      this.ViewConfig = new ViewConfig();
    }

    private class SafeDirectoryCatalog : ComposablePartCatalog
    {
      private readonly AggregateCatalog _catalog;

      public SafeDirectoryCatalog(string directory)
      {
        var files = Directory.EnumerateFiles(directory, "*.dll", SearchOption.TopDirectoryOnly);

        _catalog = new AggregateCatalog();

        foreach (var file in files.Where(f => !f.StartsWith("System.")))
        {
          try
          {
            var asmCat = new AssemblyCatalog(file);

            //Force MEF to load the plugin and figure out if there are any exports
            // good assemblies will not throw the RTLE exception and can be added to the catalog
            if (asmCat.Parts.ToList().Count > 0)
              _catalog.Catalogs.Add(asmCat);
          }
          catch (ReflectionTypeLoadException)
          {
          }
          catch (BadImageFormatException)
          {
          }
        }
      }
      public override IQueryable<ComposablePartDefinition> Parts
      {
        get { return _catalog.Parts; }
      }
    }
  }
}
