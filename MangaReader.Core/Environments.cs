using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MangaReader.Core
{
  public class Environments
  {
    [Obsolete("NOPE")]
    public static Environments Instance = new Environments();

    /// <summary>
    /// Путь к настройкам
    /// </summary>
    public string ConfigPath { get { return System.IO.Path.Combine(WorkFolder, "settings.json"); } }

    /// <summary>
    /// Название папки загрузки.
    /// </summary>
    public const string DownloadFolderName = "Download";

    /// <summary>
    /// Версия приложения.
    /// </summary>
    public Version Version { get { return typeof(Environments).Assembly.GetName().Version; } }

    /// <summary>
    /// Папка загрузки.
    /// </summary>
    public string DownloadFolder { get { return Path.Combine(WorkFolder, DownloadFolderName); } }

    /// <summary>
    /// Папка программы.
    /// </summary>
    public string WorkFolder
    {
      get { return workFolder; }
    }

    private static string workFolder = AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// Папка с либами программы.
    /// </summary>
    public string LibPath
    {
      get { return Path.Combine(WorkFolder, "lib"); }
    }

    /// <summary>
    /// Папка с плагинами программы.
    /// </summary>
    public string PluginPath
    {
      get { return Path.Combine(WorkFolder, "Plugins"); }
    }

    public string[] AssemblyFolders
    {
      get { return new string[] { LibPath, PluginPath }; }
    }
  }
}
