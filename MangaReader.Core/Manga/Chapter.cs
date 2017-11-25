using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Services;

namespace MangaReader.Core.Manga
{
  /// <summary>
  /// Глава.
  /// </summary>
  [DebuggerDisplay("{Number} {Name}")]
  public class Chapter : DownloadableContainerImpl<MangaPage>
  {
    #region Свойства

    /// <summary>
    /// Том.
    /// </summary>
    public Volume Volume { get; set; }

    /// <summary>
    /// Номер главы.
    /// </summary>
    public double Number { get; set; }

    #endregion

    public override string Folder
    {
      get { return FolderNamingStrategies.GetNamingStrategy(Volume?.Manga ?? Manga).FormateChapterFolder(this); }
    }

    public bool OnlyUpdate { get; set; }

    #region Методы

    /// <summary>
    /// Скачать главу.
    /// </summary>
    /// <param name="downloadFolder">Папка для файлов.</param>
    public override async Task Download(string downloadFolder = null)
    {
      await DownloadManager.CheckPause();
      var chapterFolder = Path.Combine(downloadFolder, this.Folder);

      if (this.Container == null || !this.Container.Any())
        this.UpdatePages();

      this.InDownloading = this.Container.ToList();
      if (this.OnlyUpdate)
      {
        await DownloadManager.CheckPause();
        this.InDownloading = History.GetItemsWithoutHistory(this);
      }

      try
      {
        await DownloadManager.CheckPause();
        chapterFolder = DirectoryHelpers.MakeValidPath(chapterFolder);
        if (!Directory.Exists(chapterFolder))
          Directory.CreateDirectory(chapterFolder);

        var pTasks = this.InDownloading.Select(page => page.Download(chapterFolder).LogException(string.Empty, $"Не удалось скачать изображение {page.ImageLink} со страницы {page.Uri}"));
        await Task.WhenAll(pTasks.ToArray());
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
    }

    /// <summary>
    /// Обновить список страниц.
    /// </summary>
    /// <remarks>Каждая конкретная глава сама забьет коллекцию this.Container.</remarks>
    protected virtual void UpdatePages()
    {
      if (this.Container == null)
        throw new ArgumentNullException(nameof(Container));
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="uri">Ссылка на главу.</param>
    /// <param name="name">Название главы.</param>
    public Chapter(Uri uri, string name) : this()
    {
      this.Uri = uri;
      this.Name = name;
    }

    protected Chapter()
    {
      
    }

    #endregion
  }
}