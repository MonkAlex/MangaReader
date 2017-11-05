using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaReader.Core.Manga
{
  public interface IDownloadableContainer<out T> : IDownloadable where T : IDownloadable
  {
    /// <summary>
    /// Загружаемое содержимое.
    /// </summary>
    IEnumerable<T> Container { get; }

    /// <summary>
    /// Содержимое после фильтрации (по истории).
    /// </summary>
    IEnumerable<T> InDownloading { get; }
  }

  public interface IDownloadable
  {

    #region Свойства

    /// <summary>
    /// Статус загрузки.
    /// </summary>
    bool IsDownloaded { get; }

    /// <summary>
    /// Процент загрузки манги.
    /// </summary>
    double Downloaded { get; set; }

    /// <summary>
    /// Ссылка на загружаемое содержимое.
    /// </summary>
    Uri Uri { get; set; }

    /// <summary>
    /// Папка с мангой.
    /// </summary>
    string Folder { get; }

    /// <summary>
    /// Загружено.
    /// </summary>
    DateTime? DownloadedAt { get; set; }

    #endregion

    #region Методы

    /// <summary>
    /// Скачать все главы.
    /// </summary>
    Task Download(string folder = null);

    /// <summary>
    /// Clear history.
    /// </summary>
    void ClearHistory();

    #endregion

  }
}
