using System;
using System.IO;
using System.Reflection;
using System.Windows;
using MangaReader.Account;

namespace MangaReader
{
  public class Settings
  {
    /// <summary>
    /// Язык манги.
    /// </summary>
    public static Languages Language = Languages.English;

    /// <summary>
    /// Обновлять при скачивании (true) или скачивать целиком(false).
    /// </summary>
    public static bool Update = true;

    /// <summary>
    /// Сворачивать в трей.
    /// </summary>
    public static bool MinimizeToTray = true;

    /// <summary>
    /// Частота автообновления библиотеки в часах.
    /// </summary>
    public static int AutoUpdateInHours = 0;

    /// <summary>
    /// Время последнего обновления.
    /// </summary>
    public static DateTime LastUpdate = DateTime.Now;

    /// <summary>
    /// Папка программы.
    /// </summary>
    public static readonly string WorkFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    /// <summary>
    /// Настройки программы.
    /// </summary>
    private static readonly string SettingsPath = WorkFolder + "\\settings.xml";

    /// <summary>
    /// Автообновление программы.
    /// </summary>
    public static bool UpdateReader = true;

    /// <summary>
    /// Папка загрузки.
    /// </summary>
    public static string DownloadFolder = WorkFolder + "\\Download\\";

    /// <summary>
    /// Префикс папки томов.
    /// </summary>
    public static string VolumePrefix = "Volume_";

    /// <summary>
    /// Префикс папки глав.
    /// </summary>
    public static string ChapterPrefix = "Chapter_";

    /// <summary>
    /// Сжимать скачанную мангу.
    /// </summary>
    public static bool CompressManga = true;

    /// <summary>
    /// Состояние окна.
    /// </summary>
    public static object[] WindowsState;

    /// <summary>
    /// Логин.
    /// </summary>
    public static Login Login = new Login();

    /// <summary>
    /// Сохранить настройки.
    /// </summary>
    public static void Save()
    {
      object[] settings = 
            {
                Language,
                Update,
                UpdateReader,
                DownloadFolder,
                CompressManga,
                WindowsState,
                new object[] {Login.Name, Login.Password},
                MinimizeToTray,
                AutoUpdateInHours
            };
      Serializer<object[]>.Save(SettingsPath, settings);
    }

    /// <summary>
    /// Загрузить настройки.
    /// </summary>
    public static void Load()
    {
      var settings = Serializer<object[]>.Load(SettingsPath);
      if (settings == null)
        return;

      try
      {
        Language = (Languages)settings[0];
        Console.WriteLine("Language {0}", Language);
        Update = (bool)settings[1];
        Console.WriteLine("Update or full download {0}", Update);
        UpdateReader = (bool)settings[2];
        Console.WriteLine("Autoupdate Mangareader {0}", UpdateReader);
        DownloadFolder = (string)settings[3];
        Console.WriteLine("Download to {0}", DownloadFolder);
        CompressManga = (bool)settings[4];
        Console.WriteLine("Need compress manga {0}", CompressManga);
        WindowsState = (object[])settings[5];
        Login = new Login() { Name = (string)((object[])settings[6])[0], Password = (string)((object[])settings[6])[1] };
        Console.WriteLine("Login {0}", Login.Name);
        MinimizeToTray = (bool)settings[7];
        Console.WriteLine("Minimize to tray {0}", MinimizeToTray);
        AutoUpdateInHours = (int)settings[8];
        Console.WriteLine("Update mangas ever {0} hours, if its not zero.", AutoUpdateInHours);
      }
      catch (IndexOutOfRangeException) { }
    }

    /// <summary>
    /// Загрузить положение и размеры окна.
    /// </summary>
    /// <param name="main">Окно.</param>
    public static void UpdateWindowsState(Window main)
    {
      if (WindowsState == null)
        return;
      try
      {
        main.Top = (double)WindowsState[0];
        main.Left = (double)WindowsState[1];
        main.Width = (double)WindowsState[2];
        main.Height = (double)WindowsState[3];
        main.WindowState = (WindowState)WindowsState[4];
      }
      catch (IndexOutOfRangeException) { }
    }

    /// <summary>
    /// Доступные языки.
    /// </summary>
    public enum Languages
    {
      English,
      Russian,
      Japanese
    }
  }
}
