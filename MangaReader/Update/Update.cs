using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;

namespace MangaReader.Services
{
  class Update
  {
    private static Uri LinkToUpdate = new Uri("https://dl.dropboxusercontent.com/u/1945107/RMG/MangaReader.exe");
    private static Uri LinkToVersion = new Uri("https://dl.dropboxusercontent.com/u/1945107/RMG/version.ini");
    private const string UpdateStarted = "update";
    private const string UpdateFinished = "updated";

    private static string UpdateFilename = Settings.WorkFolder + "\\update.exe";
    private static string UpdateTempFilename = Settings.WorkFolder + "\\update.it";
    private static string OriginalFilename = Settings.WorkFolder + "\\MangaReader.exe";
    private static string OriginalTempFilename = Settings.WorkFolder + "\\MangaReader.exe.bak";

    internal static Version ClientVersion = Settings.AppVersion;

    internal static Version ServerVersion = Settings.AppVersion;

    /// <summary>
    /// Запуск обновления, вызываемый до инициализации программы.
    /// </summary>
    /// <remarks>Завершает обновление и удаляет временные файлы.</remarks>
    internal static void Initialize()
    {
      var args = Environment.GetCommandLineArgs();
      if (args.Contains(UpdateStarted))
        Update.FinishUpdate();
      if (args.Contains(UpdateFinished))
        Update.Clean();

      if (Settings.UpdateReader)
        Update.StartUpdate();
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
    internal static void StartUpdate()
    {
      if (!Update.CheckUpdate())
        return;

      using (var client = new WebClient())
      {
        File.WriteAllBytes(UpdateFilename, client.DownloadData(LinkToUpdate));
        File.Copy(UpdateFilename, UpdateTempFilename, true);
        var run = new Process()
        {
          StartInfo =
          {
            Arguments = UpdateStarted,
            FileName = UpdateFilename,
            WorkingDirectory = Settings.WorkFolder
          }
        };
        Log.Add(string.Format("Update process started: File '{0}', Args '{1}', Folder '{2}'", UpdateFilename, UpdateStarted, Settings.WorkFolder));
        run.Start();
        Application.Current.Shutdown(1);
      }
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
          WorkingDirectory = Settings.WorkFolder
        }
      };
      Log.Add(string.Format("Update process finished: File '{0}', Args '{1}', Folder '{2}'", OriginalFilename, UpdateFinished, Settings.WorkFolder));
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
