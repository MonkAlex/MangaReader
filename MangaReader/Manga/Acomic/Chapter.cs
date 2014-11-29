using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using MangaReader.Services;

namespace MangaReader.Manga.Acomic
{
  /// <summary>
  /// Глава.
  /// </summary>
  public class Chapter
  {
    #region Свойства

    /// <summary>
    /// Количество перезапусков загрузки.
    /// </summary>
    private int restartCounter;

    /// <summary>
    /// Хранилище ссылок на изображения.
    /// </summary>
    public string ImageLink;

    /// <summary>
    /// Название главы.
    /// </summary>
    public string Name;

    /// <summary>
    /// Ссылка на главу.
    /// </summary>
    public string Url;

    /// <summary>
    /// Номер главы.
    /// </summary>
    public int Number;

    public bool IsDownloaded = false;

    #endregion

    #region Методы

    /// <summary>
    /// Скачать главу.
    /// </summary>
    /// <param name="chapterFolder">Папка для файлов.</param>
    public void Download(string chapterFolder)
    {
      this.IsDownloaded = false;

      if (restartCounter > 3)
        throw new Exception(string.Format("Load failed after {0} counts.", restartCounter));

      try
      {
        chapterFolder = Page.MakeValidPath(chapterFolder);
        if (!Directory.Exists(chapterFolder))
          Directory.CreateDirectory(chapterFolder);

        var file = Page.DownloadFile(this.ImageLink);
        if (!file.Exist)
          throw new Exception("Restart chapter download, downloaded file is corrupted, link = " + this.ImageLink);

        var fileName = Number.ToString(CultureInfo.InvariantCulture).PadLeft(4, '0') + "." + file.Extension;

        File.WriteAllBytes(string.Concat(chapterFolder, Path.DirectorySeparatorChar, fileName), file.Body);

        History.Add(this.Url);
        this.IsDownloaded = true;
      }
      catch (Exception ex)
      {
        Log.Exception(ex, this.Url, this.Name);
        ++restartCounter;
        Download(chapterFolder);
      }
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="url">Ссылка на главу.</param>
    /// <param name="desc">Описание главы.</param>
    /// <param name="link">Ссылка на изображение.</param>
    public Chapter(string url, string desc, string link)
    {
      this.Url = url;
      this.Name = desc;
      this.ImageLink = link;
      this.restartCounter = 0;
      this.Number = Convert.ToInt32(Regex.Match(url, @"/[-]?[0-9]+", RegexOptions.RightToLeft).Value.Remove(0, 1));
    }

    #endregion

  }
}