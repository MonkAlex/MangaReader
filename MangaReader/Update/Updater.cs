using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using MangaReader.Services;
using MangaReader.Services.Config;

namespace MangaReader.Update
{
  class Updater
  {
    private static Uri LinkToUpdate = new Uri("https://dl.dropboxusercontent.com/u/1945107/RMG/MangaReader.exe");
    private static Uri LinkToVersion = new Uri("https://dl.dropboxusercontent.com/u/1945107/RMG/version.ini");
    private const string UpdateStarted = "update";
    private const string UpdateFinished = "updated";

    private static string UpdateFilename = Path.Combine(ConfigStorage.WorkFolder, "update.exe");
    private static string UpdateTempFilename = Path.Combine(ConfigStorage.WorkFolder, "update.it");
    private static string OriginalFilename = Path.Combine(ConfigStorage.WorkFolder, "MangaReader.exe");
    private static string OriginalTempFilename = Path.Combine(ConfigStorage.WorkFolder, "MangaReader.exe.bak");

    internal static Version ClientVersion = AppConfig.Version;

    internal static Version ServerVersion = AppConfig.Version;

    /// <summary>
    /// Запуск обновления, вызываемый до инициализации программы.
    /// </summary>
    /// <remarks>Завершает обновление и удаляет временные файлы.</remarks>
    internal static void Initialize(bool visual)
    {
      var args = Environment.GetCommandLineArgs();
      if (args.Contains(UpdateStarted))
        Updater.FinishUpdate();
      if (args.Contains(UpdateFinished))
        Updater.Clean();

      if (ConfigStorage.Instance.AppConfig.UpdateReader)
        Updater.StartUpdate(visual);
    }

    /// <summary>
    /// Проверка наличия обновления.
    /// </summary>
    /// <returns>True, если есть обновление.</returns>
    internal static bool CheckUpdate()
    {
      try
      {
        ServerVersion = new Version(Page.GetPage(LinkToVersion));
        return ServerVersion.CompareTo(ClientVersion) > 0;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Запуск обновления.
    /// </summary>
    internal static void StartUpdate(bool visual)
    {
      if (!Updater.CheckUpdate())
        return;

      using (var client = new WebClient())
      {
        var taskBytes = client.DownloadDataTaskAsync(LinkToUpdate);
        if (visual)
        {
          var download = new Download(WindowHelper.Owner);
          client.DownloadProgressChanged += (sender, args) => download.UpdateStates(args);
          client.DownloadDataCompleted += (sender, args) => download.Close();
          download.ShowDialog();
        }
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
      Log.Add(string.Format("Update process started: File '{0}', Args '{1}', Folder '{2}'", UpdateFilename, UpdateStarted, ConfigStorage.WorkFolder));
      run.Start();
      Application.Current.Shutdown(1);
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
      Log.Add(string.Format("Update process finished: File '{0}', Args '{1}', Folder '{2}'", OriginalFilename, UpdateFinished, ConfigStorage.WorkFolder));
      run.Start();
      Application.Current.Shutdown(1);
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
      new VersionHistory().ShowDialog();
    }
  }
}
