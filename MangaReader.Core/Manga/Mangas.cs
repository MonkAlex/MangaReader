using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MangaReader.Core.Exception;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Properties;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Manga
{
  [XmlInclude(typeof(Grouple.Readmanga))/*, XmlInclude(typeof(Acomic.Acomics))*/]
  public abstract class Mangas : Entity.Entity, INotifyPropertyChanged, IDownloadable
  {
    #region Свойства

    public static Guid Type { get { return Guid.Empty; } }

    /// <summary>
    /// Название манги.
    /// </summary>
    public string Name
    {
      get { return this.IsNameChanged ? this.LocalName : this.ServerName; }
      set
      {
        if (this.IsNameChanged)
          this.LocalName = value;
        else
          this.ServerName = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(Folder));
      }
    }

    public string LocalName
    {
      get { return localName ?? ServerName; }
      set { localName = value; }
    }

    private string localName;

    public string ServerName { get; set; }

    public bool IsNameChanged
    {
      get { return isNameChanged; }
      set
      {
        isNameChanged = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Folder));
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
        UpdateUri(value);
        this.uri = value;
      }
    }

    private Uri uri;

    /// <summary>
    /// Статус манги.
    /// </summary>
    public virtual string Status
    {
      get { return status; }
      set
      {
        status = value;
        OnPropertyChanged();
      }
    }

    public bool? NeedCompress
    {
      get { return needCompress; }
      set
      {
        needCompress = value;
        OnPropertyChanged();
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
        if (Mapping.Initialized)
          return ConfigStorage.Instance.DatabaseConfig.MangaSettings.SingleOrDefault(s => Equals(s.Manga, this.GetType().TypeProperty()));
        throw new System.Exception("Mappings not initialized.");
      }
    }

    /// <summary>
    /// История манги.
    /// </summary>
    [XmlIgnore]
    public virtual IEnumerable<MangaHistory> Histories
    {
      get { return histories; }
    }

    public void AddHistory(Uri message)
    {
      AddHistory(new [] { new MangaHistory(message)});
    }

    public void AddHistory(IEnumerable<Uri> messages)
    {
      AddHistory(messages.Select(m => new MangaHistory(m)));
    }

    public void AddHistory(IEnumerable<MangaHistory> history)
    {
      lock (histories)
      {
        var list = history.Where(message => !histories.Any(h => h.Uri == message.Uri)).ToList();
        foreach (var mangaHistory in list)
        {
          histories.Add(mangaHistory);
        }
      }
    }

    public void ClearHistory()
    {
      lock (histories)
      {
        histories.Clear();
      }
    }


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
        OnPropertyChanged();
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
    private string status;
    private ICollection<MangaHistory> histories;

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
    public bool OnlyPages { get { return !this.HasVolumes && !this.HasChapters; } }

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
    public bool IsDownloaded
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
    public double Downloaded
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


    public string Folder
    {
      get { return DirectoryHelpers.MakeValidPath(Path.Combine(this.Setting.Folder, DirectoryHelpers.MakeValidPath(this.Name.Replace(Path.DirectorySeparatorChar, '.')))); }
      set { }
    }
    
    public event EventHandler<Mangas> DownloadProgressChanged;

    protected void OnDownloadProgressChanged(Mangas manga)
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

    public void Download(string mangaFolder = null)
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

      Log.AddFormat("Download start '{0}'.", this.Name);

      // Формируем путь к главе вида Папка_манги\Том_001\Глава_0001
      try
      {
        Parallel.ForEach(this.ActiveVolumes,
            v =>
            {
              v.DownloadProgressChanged += (sender, args) => this.OnPropertyChanged(nameof(Downloaded));
              v.OnlyUpdate = this.Setting.OnlyUpdate;
              v.Download(mangaFolder);
              this.AddHistory(v.ActiveChapters.Where(c => c.IsDownloaded).Select(ch => ch.Uri));
              this.AddHistory(v.ActiveChapters.SelectMany(ch => ch.ActivePages).Where(p => p.IsDownloaded).Select(p => p.Uri));
            });
        Parallel.ForEach(this.ActiveChapters,
          ch =>
          {
            ch.DownloadProgressChanged += (sender, args) => this.OnPropertyChanged(nameof(Downloaded));
            ch.OnlyUpdate = this.Setting.OnlyUpdate;
            ch.Download(mangaFolder);
            this.AddHistory(ch.Uri);
            this.AddHistory(ch.ActivePages.Where(c => c.IsDownloaded).Select(p => p.Uri));
          });
        Parallel.ForEach(this.ActivePages,
          p =>
          {
            p.DownloadProgressChanged += (sender, args) => this.OnPropertyChanged(nameof(Downloaded));
            p.Download(mangaFolder);
            this.AddHistory(p.Uri);
          });
        this.Save();
        Log.AddFormat("Download end '{0}'.", this.Name);
      }

      catch (AggregateException ae)
      {
        foreach (var ex in ae.InnerExceptions)
          Log.Exception(ex);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex);
      }
    }

    #endregion

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
    public void Compress()
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
        case null:
          throw new InvalidEnumArgumentException("CompressionMode is null", -1, typeof(Compression.CompressionMode));
        default:
          throw new InvalidEnumArgumentException(nameof(CompressionMode), (int)this.CompressionMode, typeof(Compression.CompressionMode));
      }
      Library.Status = Strings.Mangas_Compress_Completed;
    }

    protected override void BeforeSave(object[] currentState, object[] previousState, string[] propertyNames)
    {
      var dirName = previousState[propertyNames.ToList().IndexOf(nameof(Folder))] as string;
      if (dirName != null && !DirectoryHelpers.Equals(this.Folder, dirName) && Directory.Exists(dirName))
      {
        if (Directory.Exists(this.Folder))
          throw new MangaDirectoryExists("Папка уже существует.", this.Folder, this);

        // Копируем папку на новый адрес при изменении имени.
        DirectoryHelpers.MoveDirectory(dirName, this.Folder);
      }

      base.BeforeSave(currentState, previousState, propertyNames);
    }

    public override void Save()
    {
      if (!this.IsValid())
        throw new SaveValidationException("Нельзя сохранять невалидную сущность", this);

      if (Repository.Get<Mangas>().Any(m => m.Id != this.Id && (m.IsNameChanged ? m.LocalName : m.ServerName) == this.Name))
        throw new SaveValidationException("Манга с таким именем уже существует", this);

      base.Save();
    }

    public override string ToString()
    {
      return this.Name;
    }
    
    private void UpdateUri(Uri value)
    {
      if (this.uri != null && !Equals(this.uri, value))
      {
        foreach (var history in this.Histories)
        {
          var historyUri = new UriBuilder(history.Uri) { Host = value.Host, Port = -1 };
          historyUri.Path = historyUri.Path.Replace(this.uri.AbsolutePath, value.AbsolutePath);
          history.Uri = historyUri.Uri;
        }
      }
    }

    /// <summary>
    /// Создать мангу по ссылке.
    /// </summary>
    /// <param name="uri">Ссылка на мангу.</param>
    /// <returns>Манга.</returns>
    /// <remarks>Не сохранена в базе, требует заполнения полей.</remarks>
    public static Mangas Create(Uri uri)
    {
      Mangas manga = null;

      var setting  = ConfigStorage.Instance.DatabaseConfig.MangaSettings
        .SingleOrDefault(s => s.MangaSettingUris.Any(u => Equals(u.Host, uri.Host)));
      if (setting != null)
      {
        var type = Generic.GetAllTypes<Mangas>().SingleOrDefault(t => t.TypeProperty() == setting.Manga);
        if (type != null)
          manga = Activator.CreateInstance(type) as Mangas;
      }
      
      if (manga != null)
        manga.Uri = uri;

      return manga;
    }

    /// <summary>
    /// Создать мангу по ссылке, загрузив необходимую информацию с сайта.
    /// </summary>
    /// <param name="uri">Ссылка на мангу.</param>
    /// <returns>Манга.</returns>
    /// <remarks>Сохранена в базе, если была создана валидная манга.</remarks>
    public static Mangas CreateFromWeb(Uri uri)
    {
      var manga = Create(uri);
      if (manga != null)
      {
        manga.Created(uri);
        if (manga.IsValid())
          manga.Save();
      }

      return manga;
    }

    protected virtual void Created(Uri url)
    {
      this.Uri = url;
      this.Refresh();
    }

    protected Mangas()
    {
      this.histories = new List<MangaHistory>();
      this.Chapters = new List<Chapter>();
      this.Volumes = new List<Volume>();
      this.Pages = new List<MangaPage>();
    }

    #endregion
  }
}
