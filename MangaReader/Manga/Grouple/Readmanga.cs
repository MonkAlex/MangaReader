using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MangaReader.Manga.Grouple;
using MangaReader.Properties;

namespace MangaReader.Manga
{
  /// <summary>
  /// Манга.
  /// </summary>
  public class Readmanga : Mangas
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
      get { return !string.IsNullOrWhiteSpace(this.Name) && this.listOfChapters != null; }
    }

    /// <summary>
    /// Статус перевода.
    /// </summary>
    public override string IsCompleted
    {
      get
      {
        var match = Regex.Match(this.Status, Strings.Manga_IsCompleted);
        return match.Groups.Count > 1 ? match.Groups[1].Value.Trim() : null;
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
      get { return (downloadedChapters != null && downloadedChapters.Any()) ? downloadedChapters.Average(ch => ch.Downloaded) : 0; }
      set { }
    }

    /// <summary>
    /// Папка манги.
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

    /// <summary>
    /// Список глав, ссылка-описание.
    /// </summary>
    private Dictionary<string, string> listOfChapters;


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
      var page = Page.GetPage(this.Url);
      if (string.IsNullOrWhiteSpace(page))
        return;

      this.Name = Getter.GetMangaName(page).ToString();
      this.listOfChapters = Getter.GetLinksOfMangaChapters(page, this.Url);
      this.Status = Getter.GetTranslateStatus(page);
      OnPropertyChanged("IsCompleted");
    }

    public override void Compress()
    {
      Compression.CompressVolumes(this.Folder);
    }

    /// <summary>
    /// Получить список глав.
    /// </summary>
    /// <returns>Список глав.</returns>
    public List<Chapter> GetAllChapters()
    {
      if (listOfChapters == null)
        listOfChapters = Getter.GetLinksOfMangaChapters(Page.GetPage(this.Url), this.Url);
      this.allChapters = allChapters ??
             (allChapters = listOfChapters.Select(link => new Chapter(link.Key, link.Value)).ToList());
      this.allChapters.ForEach(ch => ch.DownloadProgressChanged += (sender, args) => this.DownloadProgressChanged(ch, this));
      return this.allChapters;
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
      if (volumePrefix == null)
        volumePrefix = Settings.VolumePrefix;
      if (chapterPrefix == null)
        chapterPrefix = Settings.ChapterPrefix;

      if (this.allChapters == null)
        this.GetAllChapters();

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
              ch.DownloadProgressChanged += (sender, args) => this.OnPropertyChanged("Downloaded");
              ch.Download(string.Concat(mangaFolder,
                  "\\",
                  volumePrefix,
                  ch.Volume.ToString().PadLeft(3, '0'),
                  "\\",
                  chapterPrefix,
                  ch.Number.ToString().PadLeft(4, '0')
                  ));
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
    public Readmanga(string url)
    {
      this.Url = url;
      this.Refresh();
    }

    public Readmanga() { }

    #endregion
  }
}
