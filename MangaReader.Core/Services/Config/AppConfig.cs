using System;

namespace MangaReader.Core.Services.Config
{
  public class AppConfig
  {
    /// <summary>
    /// Язык манги.
    /// </summary>
    public Languages Language { get; set; }

    /// <summary>
    /// Автообновление программы.
    /// </summary>
    public bool UpdateReader { get; set; }

    /// <summary>
    /// Сворачивать в трей.
    /// </summary>
    public bool MinimizeToTray { get; set; }

    /// <summary>
    /// Запускать программу свернутой в трей.
    /// </summary>
    public bool StartMinimizedToTray { get; set; }

    /// <summary>
    /// Частота автообновления библиотеки в часах.
    /// </summary>
    public int AutoUpdateInHours { get; set; }

    /// <summary>
    /// Время последнего обновления.
    /// </summary>
    public DateTime LastUpdate { get; set; }

    /// <summary>
    /// Префикс папки томов.
    /// </summary>
    public const string VolumePrefix = "Volume";

    /// <summary>
    /// Префикс папки глав.
    /// </summary>
    public const string ChapterPrefix = "Chapter";

    public AppConfig()
    {
      this.Language = Languages.English;
      this.UpdateReader = true;
      this.MinimizeToTray = false;
      this.StartMinimizedToTray = false;
      this.AutoUpdateInHours = 0;
      this.LastUpdate = DateTime.Now;
    }
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
