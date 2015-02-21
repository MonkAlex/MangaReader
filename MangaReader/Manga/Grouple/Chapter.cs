﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MangaReader.Services;

namespace MangaReader.Manga.Grouple
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
    private List<Uri> listOfImageLink;

    /// <summary>
    /// Название главы.
    /// </summary>
    public string Name;

    /// <summary>
    /// Ссылка на главу.
    /// </summary>
    public Uri Uri;

    /// <summary>
    /// Номер главы.
    /// </summary>
    public int Number;

    /// <summary>
    /// Номер тома.
    /// </summary>
    public int Volume;

    /// <summary>
    /// Статус загрузки.
    /// </summary>
    public bool IsDownloaded = false;

    /// <summary>
    /// Процент загрузки главы.
    /// </summary>
    public int Downloaded
    {
      get { return (this.listOfImageLink != null && this.listOfImageLink.Any()) ? _downloaded * 100 / this.listOfImageLink.Count : 0; }
    }

    private int _downloaded;

    #endregion

    public event EventHandler DownloadProgressChanged;

    protected virtual void OnDownloadProgressChanged(EventArgs e)
    {
      var handler = DownloadProgressChanged;
      if (handler != null)
        handler(this, e);
    }

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

      if (this.listOfImageLink == null)
        this.GetAllImagesLink();

      try
      {
        chapterFolder = Page.MakeValidPath(chapterFolder);
        if (!Directory.Exists(chapterFolder))
          Directory.CreateDirectory(chapterFolder);

        Parallel.ForEach(this.listOfImageLink, link =>
        {
          var file = Page.DownloadFile(link);
          if (!file.Exist)
            throw new Exception("Restart chapter download, downloaded file is corrupted, link = " + link);

          var index = this.listOfImageLink.FindIndex(l => l == link);
          var fileName = index.ToString(CultureInfo.InvariantCulture).PadLeft(4, '0') + "." + file.Extension;
          var filePath = string.Concat(chapterFolder, Path.DirectorySeparatorChar, fileName);
          file.Save(filePath);
          this._downloaded++;
          this.DownloadProgressChanged(this, null);
        });

        this.IsDownloaded = true;
      }
      catch (AggregateException ae)
      {
        foreach (var ex in ae.InnerExceptions)
          Log.Exception(ex, this.Uri.ToString(), this.Name);
        ++restartCounter;
        Download(chapterFolder);
      }
      catch (Exception ex)
      {
        Log.Exception(ex, this.Uri.ToString(), this.Name);
        ++restartCounter;
        Download(chapterFolder);
      }
    }

    /// <summary>
    /// Заполнить хранилище ссылок.
    /// </summary>
    private void GetAllImagesLink()
    {
      this.listOfImageLink = Getter.GetImagesLink(this.Uri);
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="uri">Ссылка на главу.</param>
    /// <param name="desc">Описание главы.</param>
    public Chapter(Uri uri, string desc)
    {
      this.Uri = uri;
      this.Name = desc;
      this.restartCounter = 0;
      this.Volume = Convert.ToInt32(Regex.Match(uri.ToString(), @"vol[-]?[0-9]+").Value.Remove(0, 3));
      this.Number = Convert.ToInt32(Regex.Match(uri.ToString(), @"/[-]?[0-9]+", RegexOptions.RightToLeft).Value.Remove(0, 1));
    }

    #endregion

  }
}