using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FluentNHibernate.Visitors;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.Services.Config;
using NHibernate.Linq;

namespace MangaReader.Manga
{
  [XmlInclude(typeof(Grouple.Readmanga)), XmlInclude(typeof(Acomic.Acomics))]
  public abstract class Mangas : Entity.Entity, INotifyPropertyChanged, IDownloadable
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
    public virtual Uri Uri
    {
      get { return this.uri; }
      set
      {
        if (this.uri != null && !Equals(this.uri, value))
        {
          foreach (var history in this.Histories)
          {
            var historyUri = new UriBuilder(history.Uri) { Host = value.Host };
            historyUri.Path = historyUri.Path.Replace(this.uri.AbsolutePath, value.AbsolutePath);
            historyUri.Port = -1;
            history.Uri = historyUri.Uri;
          }
        }
        this.uri = value;
      }
    }

    private Uri uri;

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
    /// Настройки манги.
    /// </summary>
    public virtual MangaSetting Setting
    {
      get
      {
        if (Mapping.Environment.Initialized)
          return ConfigStorage.Instance.DatabaseConfig.MangaSettings.SingleOrDefault(s => Equals(s.Manga, this.GetType().TypeProperty()));
        throw new Exception("Mappings not initialized.");
      }
    }

    /// <summary>
    /// История манги.
    /// </summary>
    [XmlIgnore]
    public virtual IList<MangaHistory> Histories { get; set; }

    [XmlIgnore]
    public virtual List<Volume> Volumes { get; set; }

    [XmlIgnore]
    public virtual List<Volume> ActiveVolumes { get; set; }

    [XmlIgnore]
    public virtual List<Chapter> Chapters { get; set; }

    [XmlIgnore]
    public virtual List<Chapter> ActiveChapters { get; set; }

    [XmlIgnore]
    public virtual List<MangaPage> Pages { get; set; }

    [XmlIgnore]
    public virtual List<MangaPage> ActivePages { get; set; }

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

    private static List<Compression.CompressionMode> allowedCompressionModes = 
      new List<Compression.CompressionMode>(Enum.GetValues(typeof(Compression.CompressionMode)).Cast<Compression.CompressionMode>());

    public virtual Compression.CompressionMode? CompressionMode
    {
      get
      {
        if (this.compressionMode == null)
          this.compressionMode = this.GetDefaultCompression();
        return this.compressionMode;
      }
      set
      {
        this.compressionMode = value;
      }
    }

    private Compression.CompressionMode? compressionMode;

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
    public virtual bool IsCompleted { get; set; }

    /// <summary>
    /// Признак только страниц, даже без глав.
    /// </summary>
    public virtual bool OnlyPages { get { return !this.HasVolumes && !this.HasChapters; } }

    /// <summary>
    /// Признак наличия глав.
    /// </summary>
    public virtual bool HasChapters { get; set; }

    /// <summary>
    /// Признак наличия томов.
    /// </summary>
    public virtual bool HasVolumes { get; set; }

    #endregion

    #region DownloadProgressChanged

    /// <summary>
    /// Статус загрузки.
    /// </summary>
    public virtual bool IsDownloaded
    {
      get
      {
        var isVolumesDownloaded = this.ActiveVolumes != null && this.ActiveVolumes.Any() &&
                                this.ActiveVolumes.All(v => v.IsDownloaded);
        var isChaptersDownloaded = this.ActiveChapters != null && this.ActiveChapters.Any() && this.ActiveChapters.All(v => v.IsDownloaded);
        var isPagesDownloaded = this.ActivePages != null && this.ActivePages.Any() && this.ActivePages.All(v => v.IsDownloaded);
        return isVolumesDownloaded || isChaptersDownloaded || isPagesDownloaded;
      }
    }

    /// <summary>
    /// Процент загрузки манги.
    /// </summary>
    public virtual double Downloaded
    {
      get
      {
        var volumes = (this.ActiveVolumes != null && this.ActiveVolumes.Any()) ? this.ActiveVolumes.Average(v => v.Downloaded) : double.NaN;
        var chapters = (this.ActiveChapters != null && this.ActiveChapters.Any()) ? this.ActiveChapters.Average(ch => ch.Downloaded) : double.NaN;
        var pages = (this.ActivePages != null && this.ActivePages.Any()) ? this.ActivePages.Average(ch => ch.Downloaded) : 0;
        return double.IsNaN(volumes) ? (double.IsNaN(chapters) ? pages : chapters) : volumes;
      }
      set { }
    }


    public virtual string Folder
    {
      get { return Page.MakeValidPath(Path.Combine(this.Setting.Folder, Page.MakeValidPath(this.Name.Replace(Path.DirectorySeparatorChar, '.')))); }
      set { }
    }
    
    public virtual event EventHandler<Mangas> DownloadProgressChanged;

    protected virtual void OnDownloadProgressChanged(Mangas manga)
    {
      var handler = DownloadProgressChanged;
      if (handler != null)
        handler(this, manga);
    }

    /// <summary>
    /// Обновить содержимое манги.
    /// </summary>
    /// <remarks>Каждая конкретная манга сама забьет коллекцию Volumes\Chapters\Pages.</remarks>
    protected virtual void UpdateContent()
    {
      if (this.Pages == null)
        throw new ArgumentNullException("Pages");

      if (this.Chapters == null)
        throw new ArgumentNullException("Chapters");

      if (this.Volumes == null)
        throw new ArgumentNullException("Volumes");

      this.Pages.ForEach(p => p.DownloadProgressChanged += (sender, args) => this.OnDownloadProgressChanged(this));
      this.Chapters.ForEach(ch => ch.DownloadProgressChanged += (sender, args) => this.OnDownloadProgressChanged(this));
      this.Volumes.ForEach(v => v.DownloadProgressChanged += (sender, args) => this.OnDownloadProgressChanged(this));
    }

    public virtual void Download(string mangaFolder = null)
    {
      if (!this.NeedUpdate)
        return;

      this.Refresh();

      if (mangaFolder == null)
        mangaFolder = this.Folder;

      this.UpdateContent();

      this.ActiveVolumes = this.Volumes;
      this.ActiveChapters = this.Chapters;
      this.ActivePages = this.Pages;
      if (this.Setting.OnlyUpdate)
      {
        var histories = this.Histories.ToList();

        Func<MangaPage, bool> pagesFilter = p => histories.All(m => m.Uri != p.Uri);
        Func<Chapter, bool> chaptersFilter = ch => histories.All(m => m.Uri != ch.Uri) || ch.Pages.Any(pagesFilter);
        Func<Volume, bool> volumesFilter = v => v.Chapters.Any(chaptersFilter);

        this.ActivePages = this.ActivePages.Where(pagesFilter).ToList();
        this.ActiveChapters = this.ActiveChapters.Where(chaptersFilter).ToList();
        this.ActiveVolumes = this.ActiveVolumes.Where(volumesFilter).ToList();

        histories.Clear();
      }

      if (!this.ActiveChapters.Any() && !this.ActiveVolumes.Any() && !this.ActivePages.Any())
        return;

      Log.Add("Download start " + this.Name);

      // Формируем путь к главе вида Папка_манги\Том_001\Глава_0001
      try
      {
        Parallel.ForEach(this.ActiveVolumes,
            v =>
            {
              v.DownloadProgressChanged += (sender, args) => this.OnPropertyChanged("Downloaded");
              v.OnlyUpdate = this.Setting.OnlyUpdate;
              v.Download(mangaFolder);
              this.AddHistory(v.ActiveChapters.Where(c => c.IsDownloaded).Select(ch => ch.Uri));
              this.AddHistory(v.ActiveChapters.SelectMany(ch => ch.ActivePages).Where(p => p.IsDownloaded).Select(p => p.Uri));
            });
        Parallel.ForEach(this.ActiveChapters,
          ch =>
          {
            ch.DownloadProgressChanged += (sender, args) => this.OnPropertyChanged("Downloaded");
            ch.OnlyUpdate = this.Setting.OnlyUpdate;
            ch.Download(mangaFolder);
            this.AddHistory(ch.Uri);
            this.AddHistory(ch.ActivePages.Where(c => c.IsDownloaded).Select(p => p.Uri));
          });
        Parallel.ForEach(this.ActivePages,
          p =>
          {
            p.DownloadProgressChanged += (sender, args) => this.OnPropertyChanged("Downloaded");
            p.Download(mangaFolder);
            this.AddHistory(p.Uri);
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
      Library.Status = Strings.Mangas_Compress_Started + this.Name;
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
      Library.Status = Strings.Mangas_Compress_Completed;
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

    public override string ToString()
    {
      return this.Name;
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
      if (url.Host == "hentaichan.ru")
        manga = new Hentaichan.Hentaichan(url);

      if (manga != null && manga.IsValid())
        manga.Save();

      return manga;
    }

    protected Mangas()
    {
      this.Histories = new List<MangaHistory>();
      this.Chapters = new List<Chapter>();
      this.Volumes = new List<Volume>();
      this.Pages = new List<MangaPage>();
    }

    #endregion
  }
}
