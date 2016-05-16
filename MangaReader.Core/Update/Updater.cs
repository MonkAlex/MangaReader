using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MangaReader.Core.Account;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Update
{
  public class Updater
  {
    private static Uri LinkToUpdate = new Uri("https://dl.dropboxusercontent.com/u/1945107/RMG/MangaReader.exe");
    private static Uri LinkToVersion = new Uri("https://dl.dropboxusercontent.com/u/1945107/RMG/version.ini");
    private const string UpdateStarted = "update";
    private const string UpdateFinished = "updated";

    private static string UpdateFilename = Path.Combine(ConfigStorage.WorkFolder, "update.exe");
    private static string UpdateTempFilename = Path.Combine(ConfigStorage.WorkFolder, "update.it");
    private static string OriginalFilename = Path.Combine(ConfigStorage.WorkFolder, "MangaReader.exe");
    private static string OriginalTempFilename = Path.Combine(ConfigStorage.WorkFolder, "MangaReader.exe.bak");

    public static Version ClientVersion = AppConfig.Version;

    public static Version ServerVersion = AppConfig.Version;

    /// <summary>
    /// Запуск обновления, вызываемый до инициализации программы.
    /// </summary>
    /// <remarks>Завершает обновление и удаляет временные файлы.</remarks>
    public static void Initialize(IProcess process)
    {
      var args = Environment.GetCommandLineArgs();
      if (args.Contains(UpdateStarted))
        Updater.FinishUpdate();
      if (args.Contains(UpdateFinished))
        Updater.Clean();

      if (ConfigStorage.Instance.AppConfig.UpdateReader)
      {
        process.Status = "Проверка обновлений...";
        Updater.StartUpdate(process);
      }
      else
        Log.AddFormat("Current version - {0}.", ClientVersion);
    }

    /// <summary>
    /// Проверка наличия обновления.
    /// </summary>
    /// <returns>True, если есть обновление.</returns>
    public static bool CheckUpdate()
    {
      try
      {
        ServerVersion = new Version(Page.GetPage(LinkToVersion).Content);
        var hasUpdate = ServerVersion.CompareTo(ClientVersion) > 0;
        Log.AddFormat(hasUpdate ? "Client version {0} can be updated to {1}." : "Update not found, current version - {0}.", ClientVersion, ServerVersion);
        return hasUpdate;
      }
      catch (System.Exception)
      {
        Log.AddFormat("Version check failed. Current version - {0}.", ClientVersion);
        return false;
      }
    }

    /// <summary>
    /// Запуск обновления.
    /// </summary>
    public static void StartUpdate(IProcess process)
    {
      if (!Updater.CheckUpdate())
        return;

      using (var client = new CookieClient())
      {
        process.ProgressState = ProgressState.Normal;
        var taskBytes = client.DownloadDataTaskAsync(LinkToUpdate);
        client.DownloadProgressChanged += (sender, args) =>
        {
          process.Percent = args.ProgressPercentage;
          process.Status = string.Format("{0} - {1}/{2} МБ", "Скачивается обновление", args.BytesReceived.ToMegaBytes(), args.TotalBytesToReceive.ToMegaBytes());
        };
        File.WriteAllBytes(UpdateFilename, taskBytes.Result);
      }

      File.Copy(UpdateFilename, UpdateTempFilename, true);
      var run = new Process()
      {
        StartInfo =
          {
            Arguments = UpdateStarted,
            FileName = UpdateFilename,
            WorkingDirectory = ConfigStorage.WorkFolder
          }
      };
      Log.AddFormat("Update process started: File '{0}', Args '{1}', Folder '{2}'", UpdateFilename, UpdateStarted, ConfigStorage.WorkFolder);
      run.Start();
      Environment.Exit(1);
    }

    /// <summary>
    /// Завершение обновления и запуск обновленного приложения.
    /// </summary>
    private static void FinishUpdate()
    {
      File.Replace(UpdateTempFilename, OriginalFilename, OriginalTempFilename);
      var run = new Process()
      {
        StartInfo =
        {
          Arguments = UpdateFinished,
          FileName = OriginalFilename,
          WorkingDirectory = ConfigStorage.WorkFolder
        }
      };
      Log.AddFormat("Update process finished: File '{0}', Args '{1}', Folder '{2}'", OriginalFilename, UpdateFinished, ConfigStorage.WorkFolder);
      run.Start();
      Environment.Exit(1);
    }

    /// <summary>
    /// Удаление временных файлов.
    /// </summary>
    private static void Clean()
    {
      try
      {
        File.Delete(UpdateFilename);
        File.Delete(OriginalTempFilename);
        File.Delete(UpdateTempFilename);
      }
      catch (UnauthorizedAccessException exception)
      {
        Log.Exception(exception);
      }
      Log.Add("Update process clean temporary files");
    }
  }
}
