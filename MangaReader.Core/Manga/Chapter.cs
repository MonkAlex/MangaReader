using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Exception;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Manga
{
  /// <summary>
  /// Глава.
  /// </summary>
  [DebuggerDisplay("{Number} {Name}")]
  public class Chapter : IDownloadableContainer<MangaPage>
  {
    #region Свойства

    /// <summary>
    /// Хранилище ссылок на изображения.
    /// </summary>
    public List<MangaPage> Pages { get; set; }

    /// <summary>
    /// Хранилище ссылок на изображения.
    /// </summary>
    public List<MangaPage> ActivePages { get; set; }

    public IEnumerable<MangaPage> Container { get { return this.Pages; } }

    /// <summary>
    /// Название главы.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Ссылка на главу.
    /// </summary>
    public Uri Uri { get; set; }

    /// <summary>
    /// Номер главы.
    /// </summary>
    public double Number { get; set; }

    /// <summary>
    /// Статус загрузки.
    /// </summary>
    public bool IsDownloaded
    {
      get { return this.ActivePages != null && this.ActivePages.Any() && this.ActivePages.All(v => v.IsDownloaded); }
    }
    
    /// <summary>
    /// Процент загрузки главы.
    /// </summary>
    public double Downloaded
    {
      get { return (this.ActivePages != null && this.ActivePages.Any()) ? this.ActivePages.Average(ch => ch.Downloaded) : 0; }
      set { }
    }

    #endregion

    public string Folder
    {
      get { return this.folderPrefix + this.Number.ToString(CultureInfo.InvariantCulture).PadLeft(Number % 1 == 0 ? 4 : 6, '0'); }
      private set { this.folderPrefix = value; }
    }

    public bool OnlyUpdate { get; set; }

    private string folderPrefix = AppConfig.ChapterPrefix;

    public event EventHandler<IManga> DownloadProgressChanged;

    protected void OnDownloadProgressChanged(IManga e)
    {
      var handler = DownloadProgressChanged;
      if (handler != null)
        handler(this, e);
    }

    #region Методы

    /// <summary>
    /// Скачать главу.
    /// </summary>
    /// <param name="downloadFolder">Папка для файлов.</param>
    public virtual Task Download(string downloadFolder)
    {
      var chapterFolder = Path.Combine(downloadFolder, this.Folder);

      if (this.Pages == null || !this.Pages.Any())
        this.UpdatePages();

      this.ActivePages = this.Pages;
      if (this.OnlyUpdate)
      {
        this.ActivePages = History.GetItemsWithoutHistory(this);
      }

      try
      {
        chapterFolder = DirectoryHelpers.MakeValidPath(chapterFolder);
        if (!Directory.Exists(chapterFolder))
          Directory.CreateDirectory(chapterFolder);

        var pTasks = this.ActivePages.Select(page =>
        {
          return page.Download(chapterFolder)
          .ContinueWith(t =>
            {
              if (t.Exception != null)
                Log.Exception(t.Exception, $"Не удалось скачать изображение {page.ImageLink} со страницы {page.Uri}");

              this.OnDownloadProgressChanged(null);
            });
        });
        return Task.WhenAll(pTasks.ToArray());
      }
      catch (AggregateException ae)
      {
        foreach (var ex in ae.InnerExceptions)
          Log.Exception(ex, $"Не удалось скачать главу {Name} по ссылке {Uri}");
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, $"Не удалось скачать главу {Name} по ссылке {Uri}");
      }
      return Task.FromResult(false);
    }

    /// <summary>
    /// Обновить список страниц.
    /// </summary>
    /// <remarks>Каждая конкретная глава сама забьет коллекцию this.Pages.</remarks>
    protected virtual void UpdatePages()
    {
      if (this.Pages == null)
        throw new ArgumentNullException("Pages");

      this.Pages.ForEach(p => p.DownloadProgressChanged += (sender, args) => this.OnDownloadProgressChanged(null));
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="uri">Ссылка на главу.</param>
    public Chapter(Uri uri)
    {
      this.Uri = uri;
      this.Pages = new List<MangaPage>();
      this.ActivePages = new List<MangaPage>();
    }

    #endregion
  }
}