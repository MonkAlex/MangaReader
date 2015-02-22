using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Services;

namespace MangaReader.Manga.Acomic
{
  /// <summary>
  /// Манга.
  /// </summary>
  public class Acomics : Mangas
  {
    #region Свойства

    public new static Guid Type { get { return Guid.Parse("F090B9A2-1400-4F5E-B298-18CD35341C34"); } }

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
    
    public override List<Compression.CompressionMode> AllowedCompressionModes
    {
      get { return base.AllowedCompressionModes.Where(m => Equals(m, Compression.CompressionMode.Manga)).ToList(); }
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

    #region Методы

    /// <summary>
    /// Обновить информацию о манге - название, главы, обложка.
    /// </summary>
    public override void Refresh()
    {
      var newName = Getter.GetMangaName(this.Uri);
      if (string.IsNullOrWhiteSpace(newName))
        Log.Add("Не удалось получить имя манги, текущее название = " + this.ServerName);
      else if (newName != this.ServerName)
        this.ServerName = newName;


      this.allChapters = Getter.GetMangaChapters(this.Uri);
      OnPropertyChanged("IsCompleted");
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

      if (this.allChapters == null)
        Getter.GetMangaChapters(this.Uri);

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
              ch.Download(mangaFolder);
              this.AddHistory(ch.Uri);
              this.OnPropertyChanged("Downloaded");
              OnDownloadProgressChanged(this);
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
    public Acomics(Uri url)
      : base()
    {
      this.Uri = url;
      this.ServerName = Getter.GetMangaName(url);
      this.CompressionMode = Compression.CompressionMode.Manga;
    }

    public Acomics() : base() { }

    #endregion
  }
}
