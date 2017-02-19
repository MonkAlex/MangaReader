using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Update
{
  public class Updater
  {
    private static string UpdateFilename = Path.Combine(ConfigStorage.WorkFolder, "Update", "GitHubUpdater.Launcher.exe");

    private static string UpdateConfig = Path.Combine(ConfigStorage.WorkFolder, "Update", "MangaReader.config");

    public static Version ClientVersion = AppConfig.Version;

    /// <summary>
    /// Запуск обновления, вызываемый до инициализации программы.
    /// </summary>
    /// <remarks>Завершает обновление и удаляет временные файлы.</remarks>
    public static void Initialize()
    {
      Clean();
      if (ConfigStorage.Instance.AppConfig.UpdateReader)
        StartUpdate();
      else
        Log.AddFormat("Current version - {0}.", ClientVersion);
    }

    /// <summary>
    /// Запуск обновления.
    /// </summary>
    public static void StartUpdate()
    {
      if (!File.Exists(UpdateFilename))
      {
        Log.AddFormat("Updater not found.");
        return;
      }

      var args = string.Format("--fromFile \"{0}\" --version \"{1}\" --outputFolder \"{2}\"",
        UpdateConfig, ClientVersion, ConfigStorage.WorkFolder.TrimEnd('\\'));
      Log.AddFormat("Update process started: File '{0}', Args '{1}', Folder '{2}'",
        UpdateFilename, args, ConfigStorage.WorkFolder);

      Process.Start(new ProcessStartInfo {FileName = UpdateFilename, Arguments = args});
    }

    /// <summary>
    /// Чистка файлов, используемых версией 1.38 и ниже.
    /// </summary>
    private static void Clean()
    {
      try
      {
        var x64folder = Path.Combine(ConfigStorage.WorkFolder, "x64");
        if (Directory.Exists(x64folder) && System.Environment.GetCommandLineArgs().All(a => !a.Contains("vstest")))
        {
          File.Delete(Path.Combine(ConfigStorage.WorkFolder, "sqlite3"));
          File.Delete(Path.Combine(ConfigStorage.WorkFolder, "System.Data.SQLite.dll"));
          File.Delete(Path.Combine(ConfigStorage.WorkFolder, "x86", "sqlite3.dll"));
          File.Delete(Path.Combine(x64folder, "sqlite3.dll"));
          File.Delete(Path.Combine(ConfigStorage.WorkFolder, "update.exe"));
          File.Delete(Path.Combine(ConfigStorage.WorkFolder, "update.it"));
          Directory.Delete(Path.Combine(ConfigStorage.WorkFolder, "x86"));
          Directory.Delete(x64folder);
        }
      }
      catch { }
    }
  }
}
