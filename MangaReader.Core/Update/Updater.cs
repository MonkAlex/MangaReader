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
      Log.InfoFormat("Версия приложения - {0}.", ClientVersion);
      if (ConfigStorage.Instance.AppConfig.UpdateReader)
        StartUpdate();
    }

    /// <summary>
    /// Запуск обновления.
    /// </summary>
    public static void StartUpdate()
    {
      if (!File.Exists(UpdateFilename))
      {
        Log.InfoFormat("Апдейтер не найден.");
        return;
      }

      var args = string.Format("--fromFile \"{0}\" --version \"{1}\" --outputFolder \"{2}\"",
        UpdateConfig, ClientVersion, ConfigStorage.WorkFolder.TrimEnd('\\'));
      Log.InfoFormat("Запущен процесс обновления: Файл '{0}', с аргументами '{1}', в папке '{2}'",
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
        var updateExe = Path.Combine(ConfigStorage.WorkFolder, "update.exe");
        if (File.Exists(updateExe))
        {
          File.Delete(updateExe);
          File.Delete(Path.Combine(ConfigStorage.WorkFolder, "update.it"));
        }
      }
      catch { }
    }
  }
}
