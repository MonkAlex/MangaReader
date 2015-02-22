using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MangaReader.Properties;
using MangaReader.Services;

namespace MangaReader.Manga.Grouple
{
  /// <summary>
  /// Манга.
  /// </summary>
  public class Readmanga : Mangas
  {
    #region Свойства

    public new static Guid Type { get { return Guid.Parse("2C98BBF4-DB46-47C4-AB0E-F207E283142D"); } }

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

    public override List<Compression.CompressionMode> AllowedCompressionModes
    {
      get { return base.AllowedCompressionModes.Where(m => !Equals(m, Compression.CompressionMode.Manga)).ToList(); }
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
    private Dictionary<Uri, string> listOfChapters;


    #endregion

    #region Методы

    /// <summary>
    /// Обновить информацию о манге - название, главы, обложка.
    /// </summary>
    public override void Refresh()
    {
      var page = Page.GetPage(this.Uri);
      if (string.IsNullOrWhiteSpace(page))
        return;

      var newName = Getter.GetMangaName(page).ToString();
      if (string.IsNullOrWhiteSpace(newName))
        Log.Add("Не удалось получить имя манги, текущее название = " + this.ServerName);
      else if (newName != this.ServerName)
        this.ServerName = newName;

      this.listOfChapters = Getter.GetLinksOfMangaChapters(page, this.Uri);
      this.Status = Getter.GetTranslateStatus(page);
      OnPropertyChanged("IsCompleted");
    }

    /// <summary>
    /// Получить список глав.
    /// </summary>
    /// <returns>Список глав.</returns>
    protected internal virtual List<Chapter> GetAllChapters()
    {
      if (listOfChapters == null)
        listOfChapters = Getter.GetLinksOfMangaChapters(Page.GetPage(this.Uri), this.Uri);
      this.allChapters = allChapters ??
             (allChapters = listOfChapters.Select(link => new Chapter(link.Key, link.Value)).ToList());
      this.allChapters.ForEach(ch => ch.DownloadProgressChanged += (sender, args) => OnDownloadProgressChanged(this));
      return this.allChapters;
    }

    /// <summary>
    /// Скачать все главы.
    /// </summary>
    public override void Download(string mangaFolder = null, string volumePrefix = null, string chapterPrefix = null)
    {
      if (!this.NeedUpdate)
        return;

      this.Refresh();

      if (mangaFolder == null)
        mangaFolder = this.Folder;
      if (volumePrefix == null)
        volumePrefix = Settings.VolumePrefix;
      if (chapterPrefix == null)
        chapterPrefix = Settings.ChapterPrefix;

      if (this.allChapters == null)
        this.GetAllChapters();

      this.downloadedChapters = this.allChapters;
      if (Settings.Update)
      {
        this.downloadedChapters = this.downloadedChapters
            .Where(ch => this.Histories.All(m => m.Uri != ch.Uri))
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
                  Path.DirectorySeparatorChar,
                  volumePrefix,
                  ch.Volume.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0'),
                  Path.DirectorySeparatorChar,
                  chapterPrefix,
                  ch.Number.ToString(CultureInfo.InvariantCulture).PadLeft(4, '0')
                  ));
              this.AddHistory(ch.Uri);
            });
        this.Save();
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
    public Readmanga(Uri url)
      : this()
    {
      this.Uri = url;
      this.Refresh();
    }

    public Readmanga() : base()
    {
      this.CompressionMode = Compression.CompressionMode.Volume;
    }

    #endregion
  }
}
