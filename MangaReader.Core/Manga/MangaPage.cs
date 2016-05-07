using System;
using System.Globalization;
using System.IO;
using MangaReader.Core.Services;

namespace MangaReader.Core.Manga
{
  public class MangaPage : IDownloadable
  {

    #region Свойства

    /// <summary>
    /// Количество перезапусков загрузки.
    /// </summary>
    private int restartCounter;

    /// <summary>
    /// Название страницы.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Ссылка на страницу.
    /// </summary>
    public Uri Uri { get; set; }

    /// <summary>
    /// Ссылка на изображение.
    /// </summary>
    public Uri ImageLink { get; set; }

    /// <summary>
    /// Номер страницы.
    /// </summary>
    public int Number { get; set; }

    public bool IsDownloaded { get; set; }

    public double Downloaded
    {
      get { return this.IsDownloaded ? 100 : 0; }
      set { }
    }

    public string Folder { get; private set; }

    public event EventHandler<Mangas> DownloadProgressChanged;

    protected void OnDownloadProgressChanged(Mangas e)
    {
      var handler = DownloadProgressChanged;
      if (handler != null)
        handler(this, e);
    }

    #endregion

    #region Методы

    /// <summary>
    /// Скачать страницу.
    /// </summary>
    /// <param name="chapterFolder">Папка для файлов.</param>
    public void Download(string chapterFolder)
    {
      this.IsDownloaded = false;

      if (restartCounter > 3)
        throw new System.Exception(string.Format("Load failed after {0} counts.", restartCounter));

      Library.CheckPause();
      try
      {
        chapterFolder = DirectoryHelpers.MakeValidPath(chapterFolder);
        if (!Directory.Exists(chapterFolder))
          Directory.CreateDirectory(chapterFolder);

        var file = ImageFile.DownloadFile(this.ImageLink);
        if (!file.Exist)
          throw new System.Exception("Restart download, downloaded file is corrupted, link = " + this.ImageLink);
        var fileName = this.Number.ToString(CultureInfo.InvariantCulture).PadLeft(4, '0') + "." + file.Extension;
        file.Save(Path.Combine(chapterFolder, fileName));
        this.IsDownloaded = true;
        this.OnDownloadProgressChanged(null);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, this.Uri.OriginalString);
        ++restartCounter;
        Download(chapterFolder);
      }
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="uri">Ссылка на страницу.</param>
    /// <param name="imageLink">Ссылка на изображение.</param>
    /// <param name="number">Номер страницы.</param>
    public MangaPage(Uri uri, Uri imageLink, int number)
    {
      this.Uri = uri;
      this.ImageLink = imageLink;
      this.Number = number;
      this.restartCounter = 0;
    }

    #endregion
  }
}
