using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;
using MangaReader.Manga.Acomic;
using MangaReader.Manga.Grouple;
using NHibernate.Linq;

namespace MangaReader.Services
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
    /// Версия базы данных.
    /// </summary>
    public static Version DatabaseVersion = new Version(1, 0, 0, 0);

    /// <summary>
    /// Версия приложения.
    /// </summary>
    public static Version AppVersion = Assembly.GetExecutingAssembly().GetName().Version;

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
    public static string VolumePrefix = "Volume ";

    /// <summary>
    /// Префикс папки глав.
    /// </summary>
    public static string ChapterPrefix = "Chapter ";

    [XmlIgnore]
    public static List<MangaSetting> DownloadFolders
    {
      get
      {
        if (downloadFolders == null)
        {
          var query = Mapping.Environment.Session.Query<MangaSetting>().ToList();
          downloadFolders = query.Any() ? query : GetSubclass(typeof(Manga.Mangas));
        }
        return downloadFolders;
      }
      set { }
    }

    private static List<MangaSetting> downloadFolders;

    /// <summary>
    /// Сжимать скачанную мангу.
    /// </summary>
    public static bool CompressManga = true;

    /// <summary>
    /// Состояние окна.
    /// </summary>
    public static object[] WindowsState;

    /// <summary>
    /// Сохранить настройки.
    /// </summary>
    public static void Save()
    {
      DownloadFolders.ForEach(a => a.Save());
      object[] settings = 
            {
                Language,
                Update,
                UpdateReader,
                null,
                CompressManga,
                WindowsState,
                new object[] {null, null},
                MinimizeToTray,
                AutoUpdateInHours,
                DatabaseVersion.ToString()
            };
      Serializer<object[]>.Save(SettingsPath, settings);
    }

    /// <summary>
    /// Загрузить настройки.
    /// </summary>
    public static void Load()
    {
      DownloadFolders.ForEach(a => Console.WriteLine("Type {0}, folder {1}", a.MangaName, a.Folder));

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
        CompressManga = (bool)settings[4];
        Console.WriteLine("Need compress manga {0}", CompressManga);
        WindowsState = (object[])settings[5];
        MinimizeToTray = (bool)settings[7];
        Console.WriteLine("Minimize to tray {0}", MinimizeToTray);
        AutoUpdateInHours = (int)settings[8];
        Console.WriteLine("Update mangas ever {0} hours, if its not zero.", AutoUpdateInHours);
        DatabaseVersion = new Version((string)settings[9]);
        Console.WriteLine("Curent database version = {0}.", DatabaseVersion);
      }
      catch (IndexOutOfRangeException) { }
    }

    private static List<MangaSetting> GetSubclass(Type baseClass)
    {
      var types = Assembly.GetAssembly(baseClass).GetTypes()
        .Where(type => type.IsSubclassOf(baseClass));
      var folders = types
        .Select(type => new MangaSetting
        {
          Folder = Settings.DownloadFolder,
          Manga = type.MangaType(),
          MangaName = type.Name
        })
        .ToList();
      folders.ForEach(f => f.Save());
      return folders;
    }

    public static void Convert()
    {
      var settings = Serializer<object[]>.Load(SettingsPath);
      if (settings == null)
        return;

      try
      {
        if (settings[3] is object[])
        {
          var setting = settings[3] as object[];
          var query = Mapping.Environment.Session.Query<MangaSetting>().ToList();
          if (query.FirstOrDefault(a => a.Manga == Readmanga.Type) == null)
            new MangaSetting() { Folder = setting[0] as string, Manga = Readmanga.Type, MangaName = "Readmanga" }.Save();
          if (query.FirstOrDefault(a => a.Manga == Acomics.Type) == null)
            new MangaSetting() { Folder = setting[1] as string, Manga = Acomics.Type, MangaName = "Acomics" }.Save();
        }
        Serializer<object[]>.Save(SettingsPath, settings);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
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
