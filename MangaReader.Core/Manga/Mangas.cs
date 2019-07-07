using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    /// <summary>
    /// Описание манги.
    /// </summary>
    public virtual string Description
    {
      get { return description; }
      set
      {
        description = value;
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
    public virtual MangaSetting Setting { get; protected internal set; }

    /// <summary>
    /// История манги.
    /// </summary>
    public virtual ICollection<MangaHistory> Histories { get; protected set; }

    public virtual DateTime? Created { get; set; }

    public virtual ICollection<Volume> Volumes { get; set; }

    public virtual ICollection<Volume> ActiveVolumes { get; set; }

    public virtual ICollection<Chapter> Chapters { get; set; }

    public virtual ICollection<Chapter> ActiveChapters { get; set; }

    public virtual ICollection<MangaPage> Pages { get; set; }

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
    private string description;

    /// <summary>
    /// Статус корректности манги.
    /// </summary>
    public virtual Task<bool> IsValid()
    {
      return Task.FromResult(!string.IsNullOrWhiteSpace(this.Name));
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
    public virtual async Task UpdateContent()
    {
      if (this.Pages == null)
        throw new ArgumentNullException("Pages");

      if (this.Chapters == null)
        throw new ArgumentNullException("Chapters");

      if (this.Volumes == null)
        throw new ArgumentNullException("Volumes");

      await Parser.UpdateContent(this).ConfigureAwait(false);
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
        await this.Refresh().ConfigureAwait(false);

        if (Cover == null)
          Cover = (await Parser.GetPreviews(this).ConfigureAwait(false)).FirstOrDefault();

        if (mangaFolder == null)
          mangaFolder = this.GetAbsoluteFolderPath();

        await this.UpdateContent().ConfigureAwait(false);
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
      {
        using (var context = Repository.GetEntityContext())
          await context.Save(this).ConfigureAwait(false);
        return;
      }

      Log.AddFormat("Download start '{0}'.", this.Name);

      // Формируем путь к главе вида Папка_манги\Том_001\Глава_0001
      try
      {
        using (FolderNamingStrategies.BlockStrategy(this))
        using (new Timer(state => OnPropertyChanged(nameof(Downloaded)), null, 750, 750))
        {
          NetworkSpeed.Clear();
          var plugin = Plugin;

          if (!Directory.Exists(mangaFolder))
            Directory.CreateDirectory(mangaFolder);

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
          await Task.WhenAll(tasks.Concat(chTasks).Concat(pTasks).ToArray()).ConfigureAwait(false);
          this.DownloadedAt = DateTime.Now;
          OnPropertyChanged(nameof(Downloaded));
        }

        using (var context = Repository.GetEntityContext())
          await context.Save(this).ConfigureAwait(false);
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
    public virtual async Task Refresh()
    {
      await Parser.UpdateNameAndStatus(this).ConfigureAwait(false);
      await Parser.UpdateContentType(this).ConfigureAwait(false);
      OnPropertyChanged(nameof(IsCompleted));
    }

    /// <summary>
    /// Упаковка манги.
    /// </summary>
    public virtual void Compress()
    {
      var folder = this.GetAbsoluteFolderPath();
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

    public override async Task BeforeSave(ChangeTrackerArgs args)
    {
      var debugMessage = string.Empty;
      if (!args.IsNewEntity)
      {
        var uriState = args.GetPropertyState<Uri>(nameof(Uri));
        if (uriState.IsChanged)
        {
          debugMessage += $" uri changed from '{uriState.OldValue}' to '{uriState.Value}'";
          using (Repository.GetEntityContext("Manga uri changed"))
          {
            var settings = ConfigStorage.Plugins.Where(p => p.LoginType == this.Setting.Login.GetType()).Select(p => p.GetSettings());
            var allowedUris = settings.Select(s => s.MainUri).ToList();
            if (allowedUris.Any(s => s.Host == uriState.OldValue.Host) &&
                allowedUris.All(s => s.Host != uriState.Value.Host))
              throw new MangaSaveValidationException("Нельзя менять источник манги на другой сайт.", this);
          }

          var parseResult = Parser.ParseUri(uriState.Value);
          if (!parseResult.CanBeParsed || parseResult.Kind != UriParseKind.Manga)
            throw new MangaSaveValidationException("Источник манги не поддерживается.", this);
        }
      }

      if (!await this.IsValid().ConfigureAwait(false))
        throw new MangaSaveValidationException("Нельзя сохранять невалидную сущность", this);

      if (!args.CanAddEntities)
        Log.Add(Id != 0 ? $"Save {GetType().Name} with id {Id} ({Name})." : $"New {GetType().Name} ({Name}).");

      RefreshFolder();
      args.SetPropertyState(nameof(Folder), Folder);

      if (CompressionMode == null)
      {
        CompressionMode = this.GetDefaultCompression();
        args.SetPropertyState(nameof(CompressionMode), CompressionMode);
      }

      using (var context = Repository.GetEntityContext())
      {
        if (await context.Get<IManga>().AnyAsync(m => m.Id != this.Id && m.Folder == this.Folder).ConfigureAwait(false))
          throw new MangaSaveValidationException($"Другая манга уже использует папку {this.Folder}.", this);
      }

      if (!args.IsNewEntity && !args.CanAddEntities)
      {
        var folderState = args.GetPropertyState<string>(nameof(Folder));
        if (folderState.IsChanged)
          debugMessage += $" folder changed from '{folderState.OldValue}' to '{folderState.Value}'";
        var dirName = folderState.OldValue;
        var newValue = this.GetAbsoluteFolderPath();
        var oldValue = DirectoryHelpers.GetAbsoluteFolderPath(dirName);
        if (oldValue != null && !DirectoryHelpers.Equals(newValue, oldValue) && Directory.Exists(oldValue))
        {
          if (Directory.Exists(newValue))
            throw new MangaDirectoryExists("Папка уже существует.", newValue, this);

          // Копируем папку на новый адрес при изменении имени.
          DirectoryHelpers.MoveDirectory(oldValue, newValue);
        }
      }

      if (!string.IsNullOrWhiteSpace(debugMessage) && !args.CanAddEntities)
        Log.Add($"Manga {ServerName}({Id}) changed:" + debugMessage);

      await base.BeforeSave(args).ConfigureAwait(false);
    }

    public virtual void RefreshFolder()
    {
      if (!DirectoryHelpers.ValidateSettingPath(this.Setting.Folder))
        throw new DirectoryNotFoundException($"Попытка скачивания в папку {this.Setting.Folder}, папка не существует.");

      var mangaFolder = DirectoryHelpers.RemoveInvalidCharsFromName(this.Name);
      Folder = Path.Combine(this.Setting.Folder, mangaFolder);
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
    public static async Task<IManga> Create(Uri uri)
    {
      IManga manga = null;
      MangaSetting setting;

      using (var context = Repository.GetEntityContext($"Local create manga from {uri}"))
      {
        var settings = await context.Get<MangaSetting>().ToListAsync().ConfigureAwait(false);
        setting = settings.SingleOrDefault(s => s.MainUri.Host == uri.Host);
        if (setting != null)
        {
          var plugin = ConfigStorage.Plugins.SingleOrDefault(p => Equals(p.GetSettings(), setting));
          if (plugin != null)
          {
            var parser = plugin.GetParser();
            var parseResult = parser.ParseUri(uri);
            if (parseResult != null && parseResult.CanBeParsed)
            {
              manga = Activator.CreateInstance(plugin.MangaType) as IManga;
              if (manga != null)
              {
                manga.Uri = parseResult.MangaUri;
                manga.Created = DateTime.Now;
              }
              if (manga is Mangas mangas)
                mangas.Setting = setting;
            }
          }
        }
      }

      return manga;
    }

    /// <summary>
    /// Создать мангу по ссылке, загрузив необходимую информацию с сайта.
    /// </summary>
    /// <param name="uri">Ссылка на мангу.</param>
    /// <returns>Манга.</returns>
    /// <remarks>Сохранена в базе, если была создана валидная манга.</remarks>
    public static async Task<IManga> CreateFromWeb(Uri uri)
    {
      using (var context = Repository.GetEntityContext($"Web create manga from {uri}"))
      {
        var manga = await Create(uri).ConfigureAwait(false);
        if (manga != null)
        {
          // Только для местной реализации - вызвать CreatedFromWeb\Refresh.
          if (manga is Mangas mangas)
            await mangas.CreatedFromWeb(uri).ConfigureAwait(false);

          if (await manga.IsValid().ConfigureAwait(false))
            await context.Save(manga).ConfigureAwait(false);
        }

        return manga;
      }
    }

    protected virtual async Task CreatedFromWeb(Uri url)
    {
      try
      {
        await this.Refresh().ConfigureAwait(false);
        var covers = await Parser.GetPreviews(this).ConfigureAwait(false);
        Cover = covers.FirstOrDefault();
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex);
      }
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
