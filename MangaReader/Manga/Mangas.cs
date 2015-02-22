using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using FluentNHibernate.Visitors;
using MangaReader.Services;
using NHibernate.Linq;

namespace MangaReader.Manga
{
  [XmlInclude(typeof(Grouple.Readmanga)), XmlInclude(typeof(Acomic.Acomics))]
  public class Mangas : Entity.Entity, INotifyPropertyChanged, IDownloadable
  {
    #region Свойства

    public static Guid Type { get { return Guid.Empty; } }

    /// <summary>
    /// Название манги.
    /// </summary>
    public virtual string Name
    {
      get { return this.IsNameChanged ? this.LocalName : this.ServerName; }
      set
      {
        if (this.IsNameChanged)
          this.LocalName = value;
        else
          this.ServerName = value;
        OnPropertyChanged("Name");
        OnPropertyChanged("Folder");
      }
    }

    public virtual string LocalName
    {
      get { return localName ?? ServerName; }
      set { localName = value; }
    }

    private string localName;

    public virtual string ServerName { get; set; }

    public virtual bool IsNameChanged
    {
      get { return isNameChanged; }
      set
      {
        isNameChanged = value;
        OnPropertyChanged("IsNameChanged");
        OnPropertyChanged("Name");
        OnPropertyChanged("Folder");
      }
    }

    private bool isNameChanged = false;

    public virtual string Url
    {
      get { return Uri == null ? null : Uri.ToString(); }
      set { Uri = value == null ? null : new Uri(value); }
    }

    /// <summary>
    /// Ссылка на мангу.
    /// </summary>
    [XmlIgnore]
    public virtual Uri Uri { get; set; }

    /// <summary>
    /// Статус манги.
    /// </summary>
    public virtual string Status { get; set; }

    public virtual bool? NeedCompress
    {
      get { return needCompress; }
      set
      {
        needCompress = value;
        OnPropertyChanged("NeedCompress");
      }
    }

    private bool? needCompress = null;

    /// <summary>
    /// История манги.
    /// </summary>
    [XmlIgnore]
    public virtual IList<MangaHistory> Histories { get; set; }

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

    public virtual List<Compression.CompressionMode> AllowedCompressionModes { get { return allowedCompressionModes; } }

    private static List<Compression.CompressionMode> allowedCompressionModes = new List<Compression.CompressionMode>(
      Enum.GetValues(typeof(Compression.CompressionMode))
            .Cast<Compression.CompressionMode>());

    public virtual Compression.CompressionMode? CompressionMode { get; set; }

    /// <summary>
    /// Статус корректности манги.
    /// </summary>
    public virtual bool IsValid()
    {
      return !string.IsNullOrWhiteSpace(this.Name);
    }

    /// <summary>
    /// Статус перевода.
    /// </summary>
    public virtual string IsCompleted { get; set; }

    #endregion

    #region DownloadProgressChanged

    public virtual bool IsDownloaded { get; set; }

    public virtual double Downloaded { get; set; }

    public virtual string Folder
    {
      get { return Page.MakeValidPath(DownloadFolder + this.Name); }
      set { }
    }

    public virtual string DownloadFolder
    {
      get { return Settings.DownloadFolders.Single(f => f.Manga == this.GetType().MangaType()).Folder; }
      set { }
    }

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
      return manga == null ? base.Equals(obj) : this.Uri.Equals(manga.Uri);
    }

    public override int GetHashCode()
    {
      return this.Uri.GetHashCode();
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
      switch (this.CompressionMode)
      {
        case Compression.CompressionMode.Manga:
          Compression.CompressManga(this.Folder);
          break;
        case Compression.CompressionMode.Volume:
          Compression.CompressVolumes(this.Folder);
          break;
        case Compression.CompressionMode.Chapter:
          Compression.CompressChapters(this.Folder);
          break;
        default:
          throw new InvalidEnumArgumentException("CompressionMode", 0, typeof(Compression.CompressionMode));
      }
    }

    protected override void BeforeSave(object[] currentState, object[] previousState, string[] propertyNames)
    {
      var dirName = previousState[propertyNames.ToList().IndexOf("Folder")] as string;
      if (dirName != null && this.Folder != dirName && Directory.Exists(dirName))
      {
        if (Directory.Exists(this.Folder))
          throw new DirectoryNotFoundException(
            string.Format("Папка {0} уже существует. Сохранение прервано.", this.Folder));
        if (!Page.MoveDirectory(dirName, this.Folder))
          throw new DirectoryNotFoundException(
            string.Format("Не удалось переместить {0} в {1}. Сохранение прервано.", dirName, this.Folder));
      }
      base.BeforeSave(currentState, previousState, propertyNames);
    }

    public override void Save(NHibernate.ISession session, NHibernate.ITransaction transaction)
    {
      if (!this.IsValid())
        throw new ValidationException("Нельзя сохранять невалидную сущность", "Сохранение прервано", this.GetType());

      if (session.Query<Mangas>().Any(m => m.Id != this.Id && (m.IsNameChanged ? m.LocalName : m.ServerName) == this.Name))
        throw new ValidationException("Манга с таким именем уже существует", "Сохранение прервано", this.GetType());

      base.Save(session, transaction);
    }

    /// <summary>
    /// Создать мангу по ссылке.
    /// </summary>
    /// <param name="url">Ссылка на мангу.</param>
    /// <returns>Манга.</returns>
    [Obsolete]
    public static Mangas Create(string url)
    {
      return Create(new Uri(url));
    }

    /// <summary>
    /// Создать мангу по ссылке.
    /// </summary>
    /// <param name="url">Ссылка на мангу.</param>
    /// <returns>Манга.</returns>
    public static Mangas Create(Uri url)
    {
      Mangas manga = null;

      if (url.Host == "readmanga.me" || url.Host == "adultmanga.ru")
        manga = new Grouple.Readmanga(url);
      if (url.Host == "acomics.ru")
        manga = new Acomic.Acomics(url);

      if (manga != null && manga.IsValid())
        manga.Save();

      return manga;
    }

    public Mangas()
    {
      this.Histories = new List<MangaHistory>();
    }

    #endregion
  }
}
