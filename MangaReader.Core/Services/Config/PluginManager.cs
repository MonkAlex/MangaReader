using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using MangaReader.Core.Exception;

namespace MangaReader.Core.Services.Config
{
  public class PluginManager
  {

    private IList<IPlugin> plugins;

    /// <summary>
    /// Подключенные плагины.
    /// </summary>
    public IList<IPlugin> Plugins { get { return plugins; } }

    /// <summary>
    /// Папка с плагинами программы.
    /// </summary>
    private readonly string pluginPath;
    private readonly Loader loader;

    public IPlugin GetPlugin<T>() where T : Core.Manga.IManga
    {
      return Plugins.SingleOrDefault(p => p.MangaType == typeof(T));
    }

    public void RefreshPlugins()
    {
      loader.Init();
      var result = new List<IPlugin>();

      Log.Add("Search plugins...");
      result.AddRange(GetPluginsFrom(pluginPath));
      Log.Add("Search plugins completed.");

      plugins = result;
    }

    private IEnumerable<IPlugin> GetPluginsFrom(string path)
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

              Log.Add($"Plugin {plugin.Name}-{plugin.Assembly.GetName().Version} loaded from {plugin.Assembly.Location}.");
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

    public PluginManager(string pluginFolder, Loader loader)
    {
      this.pluginPath = pluginFolder;
      this.loader = loader;
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
          catch (FileLoadException)
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
