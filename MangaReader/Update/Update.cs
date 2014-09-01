using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;

namespace MangaReader.Services
{
  class Update
  {
    private const string LinkToUpdate = "https://dl.dropboxusercontent.com/u/1945107/RMG/MangaReader.exe";
    private const string LinkToVersion = "https://dl.dropboxusercontent.com/u/1945107/RMG/version.ini";
    private const string UpdateStarted = "update";
    private const string UpdateFinished = "updated";

    private static string UpdateFilename = Settings.WorkFolder + "\\update.exe";
    private static string UpdateTempFilename = Settings.WorkFolder + "\\update.it";
    private static string OriginalFilename = Settings.WorkFolder + "\\MangaReader.exe";
    private static string OriginalTempFilename = Settings.WorkFolder + "\\MangaReader.exe.bak";

    /// <summary>
    /// Запуск обновления, вызываемый до инициализации программы.
    /// </summary>
    /// <remarks>Завершает обновление и удаляет временные файлы.</remarks>
    public static void Initialize()
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
    /// <returns></returns>
    public static bool CheckUpdate()
    {
      try
      {
        var serverVersion = new Version(Page.GetPage(LinkToVersion));
        var clientVersion = Assembly.GetExecutingAssembly().GetName().Version;
        return serverVersion.CompareTo(clientVersion) > 0;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Запуск обновления.
    /// </summary>
    public static void StartUpdate()
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
        run.Start();
        Application.Current.Shutdown(1);
      }
    }

    /// <summary>
    /// Завершение обновления и запуск обновленного приложения.
    /// </summary>
    public static void FinishUpdate()
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
      run.Start();
      Application.Current.Shutdown(1);
    }

    /// <summary>
    /// Удаление временных файлов.
    /// </summary>
    public static void Clean()
    {
      try
      {
        File.Delete(UpdateFilename);
        File.Delete(OriginalTempFilename);
        File.Delete(UpdateTempFilename);
      }
      catch (UnauthorizedAccessException exception)
      {
        Console.WriteLine(exception);
      }
      new VersionHistory().ShowDialog();
    }
  }
}
