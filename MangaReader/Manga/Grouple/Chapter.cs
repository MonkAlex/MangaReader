using System;
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
  public class Chapter : IChapter
  {
    #region Свойства

    /// <summary>
    /// Количество перезапусков загрузки.
    /// </summary>
    private int restartCounter;

    /// <summary>
    /// Хранилище ссылок на изображения.
    /// </summary>
    private List<string> listOfImageLink;

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

          if (Settings.SkipDouble && this.Parent.Doubles.Contains(file.Hash))
          {
            Console.WriteLine(file.Hash);
            var fileName = file.Hash + "." + file.Extension;
            var folder = string.Concat(chapterFolder, "\\..\\..\\Doubles\\");
            Directory.CreateDirectory(folder);
            File.WriteAllBytes(folder + fileName, file.Body);
          }
          else
          {
            this.Parent.Doubles.Add(file.Hash);
            var index = this.listOfImageLink.FindIndex(l => l == link);
            var fileName = index.ToString(CultureInfo.InvariantCulture).PadLeft(4, '0') + "." + file.Extension;
            File.WriteAllBytes(string.Concat(chapterFolder, "\\", fileName), file.Body);
          }
          this._downloaded++;
          this.DownloadProgressChanged(this, null);
        });

        this.IsDownloaded = true;
        History.Add(this.Url);
      }
      catch (AggregateException ae)
      {
        foreach (var ex in ae.InnerExceptions)
          Log.Exception(ex, this.Url, this.Name);
        ++restartCounter;
        Download(chapterFolder);
      }
      catch (Exception ex)
      {
        Log.Exception(ex, this.Url, this.Name);
        ++restartCounter;
        Download(chapterFolder);
      }
    }

    /// <summary>
    /// Заполнить хранилище ссылок.
    /// </summary>
    private void GetAllImagesLink()
    {
      this.listOfImageLink = Getter.GetImagesLink(this.Url);
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="url">Ссылка на главу.</param>
    /// <param name="desc">Описание главы.</param>
    /// <param name="parent">Ссылка на мангу.</param>
    public Chapter(string url, string desc, Mangas parent)
    {
      this.Url = url;
      this.Name = desc;
      this.restartCounter = 0;
      this.Volume = Convert.ToInt32(Regex.Match(url, @"vol[-]?[0-9]+").Value.Remove(0, 3));
      this.Number = Convert.ToInt32(Regex.Match(url, @"/[-]?[0-9]+", RegexOptions.RightToLeft).Value.Remove(0, 1));
      this.Parent = parent;
    }

    #endregion

    public Mangas Parent { get; set; }
  }
}