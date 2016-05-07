using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using MangaReader.Core.Properties;

namespace MangaReader.Manga
{
  /// <summary>
  /// Манга.
  /// </summary>
  [Obsolete]
  public class Manga : INotifyPropertyChanged
  {
    #region Свойства

    /// <summary>
    /// Название манги.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Ссылка на мангу.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Статус манги.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Нужно ли обновлять мангу.
    /// </summary>
    public bool NeedUpdate
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
    /// Обложка.
    /// </summary>
    public byte[] Cover { get; set; }

    /// <summary>
    /// Статус корректности манги.
    /// </summary>
    public bool IsValid
    {
      get { return !string.IsNullOrWhiteSpace(this.Name); }
    }

    /// <summary>
    /// Папка с мангой.
    /// </summary>
    public string Folder
    {
      get
      {
        throw new NotImplementedException("Obsolete class");
      }
    }

    /// <summary>
    /// Статус перевода.
    /// </summary>
    public string IsCompleted
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
    public bool IsDownloaded
    {
      get { return false; }
    }

    /// <summary>
    /// Процент загрузки манги.
    /// </summary>
    public double Downloaded
    {
      get { return 0; }
      set { }
    }

    #endregion

    public event EventHandler<Manga> DownloadProgressChanged;

    protected virtual void OnDownloadProgressChanged(Manga manga)
    {
      var handler = DownloadProgressChanged;
      if (handler != null)
        handler(this, manga);
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string property)
    {
      var handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(property));
      }
    }

    #endregion

    #region Методы

    /// <summary>
    /// Обновить информацию о манге - название, главы, обложка.
    /// </summary>
    public void Refresh()
    {
      throw new NotImplementedException("Obsolete class");
    }

    /// <summary>
    /// Скачать все главы.
    /// </summary>
    public void Download(string mangaFolder = null, string volumePrefix = null, string chapterPrefix = null)
    {
      throw new NotImplementedException("Obsolete class");
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
    public Manga(string url)
    {
      this.Url = url;
      this.Refresh();
    }

    public Manga() { }

    #endregion
  }
}
