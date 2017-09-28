using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MangaReader.Core.Manga
{
  public interface IDownloadableContainer<T> : IDownloadable where T : IDownloadable
  {
    /// <summary>
    /// Загружаемое содержимое.
    /// </summary>
    ICollection<T> Container { get; }

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

    #endregion

    #region DownloadProgressChanged

    event EventHandler<IManga> DownloadProgressChanged;

    #endregion

    #region Методы

    /// <summary>
    /// Скачать все главы.
    /// </summary>
    Task Download(string folder = null);

    #endregion

  }
}
