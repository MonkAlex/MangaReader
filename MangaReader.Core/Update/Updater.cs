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
      if (ConfigStorage.Instance.AppConfig.UpdateReader)
      {
        StartUpdate();
      }
      else
        Log.AddFormat("Current version - {0}.", ClientVersion);
    }

    /// <summary>
    /// Запуск обновления.
    /// </summary>
    public static void StartUpdate()
    {
      var args = string.Join(" ", 
        "--fromFile", UpdateConfig, 
        "--version", ClientVersion, 
        "--outputFolder", ConfigStorage.WorkFolder);
      Log.AddFormat("Update process started: File '{0}', Args '{1}', Folder '{2}'", 
        UpdateFilename, args, ConfigStorage.WorkFolder);

      Process.Start(new ProcessStartInfo { FileName = UpdateFilename, Arguments = args });
    }
  }
}
