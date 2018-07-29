using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MangaReader.Core.Entity;
using MangaReader.Core.Exception;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Properties;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Manga
{
  [DebuggerDisplay("{Name}, Id = {Id}, Uri = {Uri}")]
  public abstract class Mangas : Entity.Entity, IManga
  {
    #region Свойства

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
        OnPropertyChanged();
        OnPropertyChanged(nameof(Folder));
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
        OnPropertyChanged();
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Folder));
      }
    }

    private bool isNameChanged = false;

    public virtual string Url
    {
      get { return Uri?.ToString(); }
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

    public virtual bool? NeedCompress
    {
      get { return needCompress; }
      set
      {
        needCompress = value;
        OnPropertyChanged();
      }
    }

    private bool? needCompress = null;

    protected internal virtual IPlugin Plugin
    {
      get
      {
        var plugin = ConfigStorage.Plugins.SingleOrDefault(p => p.MangaType == this.GetType());
        if (plugin == null)
          throw new MangaReaderException(string.Format("Plugin for {0} manga type not found.", this.GetType()));
        return plugin;
      }
    }

    public virtual ISiteParser Parser
    {
      get
      {
        if (Mapping.Initialized)
          return Plugin.GetParser();
        throw new MangaReaderException("Mappings not initialized.");
      }
    }

    /// <summary>
    /// Настройки манги.
    /// </summary>
    [XmlIgnore]
    public virtual MangaSetting Setting { get; protected internal set; }

    /// <summary>
    /// История манги.
    /// </summary>
    [XmlIgnore]
    public virtual ICollection<MangaHistory> Histories { get; protected set; }

    public virtual DateTime? Created { get; set; }

    [XmlIgnore]
    public virtual ICollection<Volume> Volumes { get; set; }

    [XmlIgnore]
    public virtual ICollection<Volume> ActiveVolumes { get; set; }

    [XmlIgnore]
    public virtual ICollection<Chapter> Chapters { get; set; }

    [XmlIgnore]
    public virtual ICollection<Chapter> ActiveChapters { get; set; }

    [XmlIgnore]
    public virtual ICollection<MangaPage> Pages { get; set; }

    [XmlIgnore]
    public virtual ICollection<MangaPage> ActivePages { get; set; }

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

    public virtual Compression.CompressionMode? CompressionMode { get; set; }

    private string status;
    private byte[] cover;

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

    #region Download

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

    public virtual string Folder { get; set; }

    public virtual DateTime? DownloadedAt { get; set; }

    public virtual byte[] Cover
    {
      get { return cover; }
      set
      {
        cover = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Обновить содержимое манги.
    /// </summary>
    /// <remarks>Каждая конкретная манга сама забьет коллекцию Volumes\Chapters\Pages.</remarks>
    public virtual void UpdateContent()
    {
      if (this.Pages == null)
        throw new ArgumentNullException("Pages");

      if (this.Chapters == null)
        throw new ArgumentNullException("Chapters");

      if (this.Volumes == null)
        throw new ArgumentNullException("Volumes");

      Parser.UpdateContent(this);
    }

    protected void AddToHistory(params IDownloadable[] downloadables)
    {
      foreach (var downloadable in downloadables)
      {
        Histories.Add(new MangaHistory(downloadable.Uri));
        if (downloadable.DownloadedAt == null)
          downloadable.DownloadedAt = DateTime.Now;
      }
    }

    public virtual async Task Download(string mangaFolder = null)
    {
      if (!this.NeedUpdate)
        return;

      try
      {
        this.Refresh();

        if (Cover == null)
          Cover = Parser.GetPreviews(this).FirstOrDefault();

        if (mangaFolder == null)
          mangaFolder = this.GetAbsoulteFolderPath();

        this.UpdateContent();
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, $"Не удалось получить информацию о манге {Name} ({Uri})");
      }

      this.ActiveVolumes = this.Volumes;
      this.ActiveChapters = this.Chapters;
      this.ActivePages = this.Pages;

      if (this.Setting.OnlyUpdate)
      {
        History.FilterActiveElements(this);
      }

      if (!this.ActiveChapters.Any() && !this.ActiveVolumes.Any() && !this.ActivePages.Any())
        return;

      Log.AddFormat("Download start '{0}'.", this.Name);

      // Формируем путь к главе вида Папка_манги\Том_001\Глава_0001
      try
      {
        using (FolderNamingStrategies.BlockStrategy(this))
        using (new Timer(state => OnPropertyChanged(nameof(Downloaded)), null, 750, 750))
        {
          NetworkSpeed.Clear();
          var plugin = Plugin;
          var tasks = this.ActiveVolumes.Select(
              v =>
              {
                v.OnlyUpdate = this.Setting.OnlyUpdate;
                return v.Download(mangaFolder).ContinueWith(t =>
                {
                  if (t.Exception != null)
                    Log.Exception(t.Exception, v.Uri?.ToString());

                  if (plugin.HistoryType == HistoryType.Chapter)
                    AddToHistory(v.InDownloading.Where(c => c.IsDownloaded).ToArray());

                  if (plugin.HistoryType == HistoryType.Page)
                    AddToHistory(v.InDownloading.SelectMany(ch => ch.InDownloading).Where(p => p.IsDownloaded).ToArray());
                });
              });
          var chTasks = this.ActiveChapters.Select(
            ch =>
            {
              ch.OnlyUpdate = this.Setting.OnlyUpdate;
              return ch.Download(mangaFolder).ContinueWith(t =>
              {
                if (t.Exception != null)
                  Log.Exception(t.Exception, ch.Uri?.ToString());

                if (ch.IsDownloaded && plugin.HistoryType == HistoryType.Chapter)
                  AddToHistory(ch);

                if (plugin.HistoryType == HistoryType.Page)
                  AddToHistory(ch.InDownloading.Where(c => c.IsDownloaded).ToArray());
              });
            });
          var pTasks = this.ActivePages.Select(
            p =>
            {
              return p.Download(mangaFolder).ContinueWith(t =>
              {
                if (t.Exception != null)
                  Log.Exception(t.Exception, $"Не удалось скачать изображение {p.ImageLink} со страницы {p.Uri}");
                if (p.IsDownloaded && plugin.HistoryType == HistoryType.Page)
                  AddToHistory(p);
              });
            });
          await Task.WhenAll(tasks.Concat(chTasks).Concat(pTasks).ToArray());
          this.DownloadedAt = DateTime.Now;
          OnPropertyChanged(nameof(Downloaded));
        }

        using (var context = Repository.GetEntityContext())
          context.Save(this);
        NetworkSpeed.Clear();
        Log.AddFormat("Download end '{0}'.", this.Name);
      }

      catch (AggregateException ae)
      {
        foreach (var ex in ae.Flatten().InnerExceptions)
          Log.Exception(ex);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex);
      }
    }

    public virtual void ClearHistory()
    {
      Histories.Clear();

      foreach (var volume in Volumes)
        volume.ClearHistory();
      foreach (var chapter in Chapters)
        chapter.ClearHistory();
      foreach (var page in Pages)
        page.ClearHistory();
      this.DownloadedAt = null;
    }

    #endregion

    #region INotifyPropertyChanged

    public virtual event PropertyChangedEventHandler PropertyChanged;

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
      Parser.UpdateNameAndStatus(this);
      Parser.UpdateContentType(this);
      OnPropertyChanged(nameof(IsCompleted));
    }

    /// <summary>
    /// Упаковка манги.
    /// </summary>
    public virtual void Compress()
    {
      var folder = this.GetAbsoulteFolderPath();
      if (!Directory.Exists(folder))
        return;

      Log.Info(Strings.Mangas_Compress_Started + this.Name);
      switch (this.CompressionMode)
      {
        case Compression.CompressionMode.Manga:
          Compression.CompressManga(folder);
          break;
        case Compression.CompressionMode.Volume:
          Compression.CompressVolumes(folder);
          break;
        case Compression.CompressionMode.Chapter:
          Compression.CompressChapters(folder);
          break;
        case null:
          throw new InvalidEnumArgumentException("CompressionMode is null", -1, typeof(Compression.CompressionMode));
        default:
          throw new InvalidEnumArgumentException(nameof(CompressionMode), (int)this.CompressionMode, typeof(Compression.CompressionMode));
      }
      Log.Info(Strings.Mangas_Compress_Completed);
    }

    public override void BeforeSave(ChangeTrackerArgs args)
    {
      if (!args.CanAddEntities)
        Log.Add(Id != 0 ? $"Save {GetType().Name} with id {Id} ({Name})." : $"New {GetType().Name} ({Name}).");

      if (!this.IsValid())
        throw new SaveValidationException("Нельзя сохранять невалидную сущность", this);

      RefreshFolder();
      args.CurrentState[args.PropertyNames.ToList().IndexOf(nameof(Folder))] = Folder;

      if (CompressionMode == null)
      {
        CompressionMode = this.GetDefaultCompression();
        args.CurrentState[args.PropertyNames.ToList().IndexOf(nameof(CompressionMode))] = CompressionMode;
      }

      using (var context = Repository.GetEntityContext())
      {
        if (context.Get<IManga>().Any(m => m.Id != this.Id && m.Folder == this.Folder))
          throw new SaveValidationException($"Другая манга уже использует папку {this.Folder}.", this);
      }

      if (args.PreviousState != null)
      {
        var dirName = args.PreviousState[args.PropertyNames.ToList().IndexOf(nameof(Folder))] as string;
        dirName = DirectoryHelpers.MakeValidPath(dirName);
        var newValue = this.GetAbsoulteFolderPath();
        var oldValue = DirectoryHelpers.GetAbsoulteFolderPath(dirName);
        if (oldValue != null && !DirectoryHelpers.Equals(newValue, oldValue) && Directory.Exists(oldValue))
        {
          if (Directory.Exists(newValue))
            throw new MangaDirectoryExists("Папка уже существует.", newValue, this);

          // Копируем папку на новый адрес при изменении имени.
          DirectoryHelpers.MoveDirectory(oldValue, newValue);
        }
      }

      base.BeforeSave(args);
    }

    public virtual void RefreshFolder()
    {
      var mangaFolder = DirectoryHelpers.MakeValidPath(this.Name.Replace(Path.DirectorySeparatorChar, '.'));
      Folder = DirectoryHelpers.MakeValidPath(Path.Combine(this.Setting.Folder, mangaFolder));
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
          var historyUri = new UriBuilder(history.Uri) { Scheme = value.Scheme, Host = value.Host, Port = -1 };
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
    public static IManga Create(Uri uri)
    {
      IManga manga = null;
      MangaSetting setting;

      using (var context = Repository.GetEntityContext())
      {
        setting = context.Get<MangaSetting>().ToList().SingleOrDefault(s => s.MangaSettingUris.Any(u => u.Host == uri.Host));
        if (setting != null)
        {
          var plugin = ConfigStorage.Plugins.SingleOrDefault(p => Equals(p.GetSettings(), setting));
          if (plugin != null)
          {
            manga = Activator.CreateInstance(plugin.MangaType) as IManga;
            if (manga != null)
              manga.Created = DateTime.Now;
            if (manga is Mangas mangas)
              mangas.Setting = setting;
          }
        }
      }

      if (manga != null)
      {
        var parseResult = manga.Parser.ParseUri(uri);
        manga.Uri = parseResult.CanBeParsed ? parseResult.MangaUri : uri;
      }

      return manga;
    }

    /// <summary>
    /// Создать мангу по ссылке, загрузив необходимую информацию с сайта.
    /// </summary>
    /// <param name="uri">Ссылка на мангу.</param>
    /// <returns>Манга.</returns>
    /// <remarks>Сохранена в базе, если была создана валидная манга.</remarks>
    public static IManga CreateFromWeb(Uri uri)
    {
      using (var context = Repository.GetEntityContext())
      {
        var manga = Create(uri);
        if (manga != null)
        {
          // Только для местной реализации - вызвать CreatedFromWeb\Refresh.
          if (manga is Mangas mangas)
            mangas.CreatedFromWeb(uri);

          if (manga.IsValid())
            context.Save(manga);
        }

        return manga;
      }
    }

    protected virtual void CreatedFromWeb(Uri url)
    {
      this.Refresh();
    }

    protected void AddHistoryReadedUris<T>(T source, Uri url) where T : IEnumerable<IDownloadable>
    {
      var readed = source.TakeWhile(c => c.Uri.AbsolutePath != url.AbsolutePath).ToArray();
      AddToHistory(readed);
    }

    protected Mangas()
    {
      this.Histories = new HistoryCollection();
      this.Chapters = new List<Chapter>();
      this.Volumes = new List<Volume>();
      this.Pages = new List<MangaPage>();
    }

    #endregion
  }
}
