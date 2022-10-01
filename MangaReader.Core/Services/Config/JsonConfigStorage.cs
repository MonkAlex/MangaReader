using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MangaReader.Core.Exception;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MangaReader.Core.Services.Config
{
  public class JsonConfigStorage
  {

    /// <summary>
    /// Путь к настройкам программы.
    /// </summary>
    private readonly string path;

    /// <summary>
    /// Загруженный экземпляр настроек.
    /// </summary>
    private Config loadedConfig;

    public Config Load()
    {
      if (loadedConfig != null)
        return loadedConfig;

      JsonConvert.DefaultSettings = () =>
      new JsonSerializerSettings
      {
        Formatting = Formatting.Indented,
        Converters = new List<JsonConverter> { new StringEnumConverter(), new VersionConverter() }
      };

      loadedConfig = null;
      try
      {
        if (File.Exists(path))
        {
          loadedConfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path, Encoding.UTF8));
        }
      }
      catch (System.Exception e)
      {
        Log.Exception(e, "Fail load settings.");
      }

      if (loadedConfig == null)
      {
        loadedConfig = new Config();
        Log.Add("Settings not found, create new default settings file.");
      }

      return loadedConfig;
    }

    public void Save()
    {
      var str = JsonConvert.SerializeObject(loadedConfig);
      File.WriteAllText(path, str, Encoding.UTF8);
      Log.Add($"Settings saved to {path}.");
    }

    public JsonConfigStorage(string path)
    {
      this.path = path;
    }
  }
}
