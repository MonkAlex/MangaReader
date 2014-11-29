using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using MangaReader.Services;

namespace MangaReader.Manga
{
  [XmlInclude(typeof(Grouple.Readmanga)), XmlInclude(typeof(Acomic.Acomics))]
  public class Mangas : Entity.Entity, INotifyPropertyChanged, IDownloadable
  {
    #region Свойства

    protected static internal string Type { get { return "Type"; } }

    /// <summary>
    /// Название манги.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Ссылка на мангу.
    /// </summary>
    public virtual string Url { get; set; }

    /// <summary>
    /// Статус манги.
    /// </summary>
    public virtual string Status { get; set; }

    public virtual List<ImageFile> Files { get; set; }

    public virtual List<string> Doubles
    {
      get { return doubles; }
      set { doubles = value; }
    }

    public virtual List<string> Extend { get; set; }

    /// <summary>
    /// Нужно ли обновлять мангу.
    /// </summary>
    public virtual bool NeedUpdate
    {
      get { return needUpdate; }
      set
      {
        needUpdate = value;
        OnPropertyChanged("NeedUpdate");
      }
    }

    private bool needUpdate = true;

    private List<string> doubles = new List<string>();

    /// <summary>
    /// Статус корректности манги.
    /// </summary>
    public virtual bool IsValid()
    {
      return true;
    }

    /// <summary>
    /// Статус перевода.
    /// </summary>
    public virtual string IsCompleted { get; set; }

    #endregion

    #region DownloadProgressChanged

    public virtual bool IsDownloaded { get; set; }

    public virtual double Downloaded { get; set; }

    public virtual string Folder { get; set; }

    internal static string DownloadFolder
    {
      get { return string.IsNullOrWhiteSpace(downloadFolder) ? Settings.DownloadFolder : downloadFolder; }
      set { downloadFolder = value; }
    }

    private static string downloadFolder;

    public virtual event EventHandler<Mangas> DownloadProgressChanged;

    protected virtual void OnDownloadProgressChanged(Mangas manga)
    {
      var handler = DownloadProgressChanged;
      if (handler != null)
        handler(this, manga);
    }

    public virtual void Download(string mangaFolder = null, string volumePrefix = null, string chapterPrefix = null)
    {
      
    }

    #endregion

    #region INotifyPropertyChanged

    public virtual event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string property)
    {
      var handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(property));
      }
    }

    #endregion

    #region Equals

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;

      var manga = obj as Mangas;
      return manga == null ? base.Equals(obj) : this.Url.Equals(manga.Url);
    }

    public override int GetHashCode()
    {
      return this.Url.GetHashCode();
    }

    #endregion
    
    #region Методы

    /// <summary>
    /// Обновить информацию о манге - название, главы, обложка.
    /// </summary>
    public virtual void Refresh()
    {
      
    }

    /// <summary>
    /// Упаковка манги.
    /// </summary>
    public virtual void Compress()
    {
      
    }

    /// <summary>
    /// Создать мангу по ссылке.
    /// </summary>
    /// <param name="url">Ссылка на мангу.</param>
    /// <returns>Манга.</returns>
    public static Mangas Create(string url)
    {
      Mangas manga = null;

      if (url.Contains("readmanga.me") || url.Contains("adultmanga.ru"))
        manga = new Grouple.Readmanga(url);
      if (url.Contains("acomics.ru"))
        manga = new Acomic.Acomics(url);

      if (manga != null)
        manga.Save();

      return manga;
    }

    #endregion
  }
}
