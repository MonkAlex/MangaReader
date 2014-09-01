using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Manga.Acomic;

namespace MangaReader.Manga
{
  /// <summary>
  /// Манга.
  /// </summary>
  public class Acomics : Mangas
  {
    #region Свойства

    /// <summary>
    /// Статус манги.
    /// </summary>
    public override string Status { get; set; }

    /// <summary>
    /// Нужно ли обновлять мангу.
    /// </summary>
    public override bool NeedUpdate
    {
      get { return _needUpdate; }
      set
      {
        _needUpdate = value;
        OnPropertyChanged("NeedUpdate");
      }
    }

    private bool _needUpdate = true;

    /// <summary>
    /// Статус корректности манги.
    /// </summary>
    public override bool IsValid
    {
      get { return !string.IsNullOrWhiteSpace(this.Name); }
    }

    /// <summary>
    /// Статус перевода.
    /// </summary>
    public override string IsCompleted
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Статус загрузки.
    /// </summary>
    public override bool IsDownloaded
    {
      get { return downloadedChapters != null && downloadedChapters.Any() && downloadedChapters.All(c => c.IsDownloaded); }
    }

    /// <summary>
    /// Процент загрузки манги.
    /// </summary>
    public override double Downloaded
    {
      get { return (downloadedChapters != null && downloadedChapters.Any()) ? (downloadedChapters.Count(ch => ch.IsDownloaded) / (double)downloadedChapters.Count) * 100.0 : 0; }
      set { }
    }

    /// <summary>
    /// Папка типа манги.
    /// </summary>
    public override string Folder
    {
      get { return Page.MakeValidPath(DownloadFolder + this.Name); }
    }

    /// <summary>
    /// Загружаемый список глав.
    /// </summary>
    private List<Chapter> downloadedChapters;

    /// <summary>
    /// Закешированный список глав.
    /// </summary>
    private List<Chapter> allChapters;


    #endregion

    #region DownloadProgressChanged

    public override event EventHandler<Mangas> DownloadProgressChanged;

    internal static string DownloadFolder
    {
      get { return string.IsNullOrWhiteSpace(downloadFolder) ? Settings.DownloadFolder : downloadFolder; }
      set { downloadFolder = value; }
    }

    private static string downloadFolder;

    protected virtual void OnDownloadProgressChanged(Mangas manga)
    {
      var handler = DownloadProgressChanged;
      if (handler != null)
        handler(this, manga);
    }

    #endregion

    #region Методы

    /// <summary>
    /// Обновить информацию о манге - название, главы, обложка.
    /// </summary>
    public override void Refresh()
    {
      this.Name = Getter.GetMangaName(this.Url);
      this.allChapters = Getter.GetMangaChapters(this.Url);
      OnPropertyChanged("IsCompleted");
    }

    public override void Compress()
    {
      Compression.CompressManga(this.Folder);
    }

    /// <summary>
    /// Скачать все главы.
    /// </summary>
    public override void Download(string mangaFolder = null, string volumePrefix = null, string chapterPrefix = null)
    {
      if (!this.NeedUpdate)
        return;

      if (mangaFolder == null)
        mangaFolder = this.Folder;

      if (this.allChapters == null)
        Getter.GetMangaChapters(this.Url);

      this.downloadedChapters = this.allChapters;
      if (Settings.Update == true)
      {
        var messages = History.Get(this.Url);
        this.downloadedChapters = this.downloadedChapters
            .Where(ch => messages.All(m => m.Url != ch.Url))
            .ToList();
      }

      if (!this.downloadedChapters.Any())
        return;

      Log.Add("Download start " + this.Name);

      // Формируем путь к главе вида Папка_манги\Том_001\Глава_0001
      try
      {
        Parallel.ForEach(this.downloadedChapters,
            ch =>
            {
              ch.Download(mangaFolder);
              this.OnPropertyChanged("Downloaded");
              this.DownloadProgressChanged(ch, this);
            });
        Log.Add("Download end " + this.Name);
      }

      catch (AggregateException ae)
      {
        foreach (var ex in ae.InnerExceptions)
          Log.Exception(ex);
      }
      catch (Exception ex)
      {
        Log.Exception(ex);
      }
    }

    public override string ToString()
    {
      return this.Name;
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Открыть мангу.
    /// </summary>
    /// <param name="url">Ссылка на мангу.</param>
    public Acomics(string url)
    {
      this.Url = url;
      this.Name = Getter.GetMangaName(this.Url);
    }

    public Acomics() { }

    #endregion
  }
}
