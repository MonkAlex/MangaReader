using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Properties;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{
  public class LibraryViewModel : INotifyPropertyChanged
  {
    /// <summary>
    /// Таймер для автообновления манги.
    /// </summary>
    private readonly Timer Timer = null;

    private int mangaIndex;
    private int mangasCount;
    private bool isAvaible = true;
    private bool shutdownPc;

    /// <summary>
    /// Признак паузы.
    /// </summary>
    public bool IsPaused
    {
      get { return DownloadManager.IsPaused; }
      set
      {
        DownloadManager.IsPaused = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Библиотека доступна, т.е. не в процессе обновления.
    /// </summary>
    public bool IsAvaible
    {
      get { return isAvaible; }
      private set
      {
        isAvaible = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(InProcess));
      }
    }

    public bool InProcess
    {
      get { return !IsAvaible; }
    }

    public bool ShutdownPC
    {
      get { return shutdownPc; }
      set
      {
        shutdownPc = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Выполнить тяжелое действие изменения библиотеки в отдельном потоке.
    /// </summary>
    /// <param name="action">Выполняемое действие.</param>
    /// <remarks>Только одно действие за раз. Доступность выполнения можно проверить в IsAvaible.</remarks>
    public async Task ThreadAction(Action action)
    {
      if (!IsAvaible)
        throw new MangaReaderException("Library not avaible.");

      IsAvaible = false;
      await Task.Run(action);
      IsAvaible = true;
    }

    #region Методы

    /// <summary>
    /// Добавить мангу.
    /// </summary>
    /// <param name="url"></param>
    public bool Add(string url)
    {
      if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri) && Add(uri))
        return true;

      Log.Info("Некорректная ссылка.");
      return false;
    }

    /// <summary>
    /// Добавить мангу.
    /// </summary>
    /// <param name="uri"></param>
    public bool Add(Uri uri)
    {
      if (Repository.Get<IManga>().Any(m => m.Uri == uri))
        return false;

      var newManga = Mangas.CreateFromWeb(uri);
      if (newManga == null || !newManga.IsValid())
        return false;

      OnLibraryChanged(new LibraryViewModelArgs(null, newManga, MangaOperation.Added, LibraryOperation.UpdateMangaChanged));
      Log.Info(Strings.Library_Status_MangaAdded + newManga.Name);
      return true;
    }

    /// <summary>
    /// Удалить мангу.
    /// </summary>
    /// <param name="manga"></param>
    public void Remove(IManga manga)
    {
      if (manga == null)
        return;

      manga.Delete();
      OnLibraryChanged(new LibraryViewModelArgs(null, manga, MangaOperation.Deleted, LibraryOperation.UpdateMangaChanged));

      Log.Info(Strings.Library_Status_MangaRemoved + manga.Name);
    }

    private void TimerTick(object sender)
    {
      if (IsAvaible && ConfigStorage.Instance.AppConfig.AutoUpdateInHours > 0 &&
          DateTime.Now > ConfigStorage.Instance.AppConfig.LastUpdate.AddHours(ConfigStorage.Instance.AppConfig.AutoUpdateInHours))
      {
        Log.InfoFormat("{0} Время последнего обновления - {1}, частота обновления - каждые {2} часов.",
          Strings.AutoUpdate, ConfigStorage.Instance.AppConfig.LastUpdate, ConfigStorage.Instance.AppConfig.AutoUpdateInHours);

        if (IsAvaible)
        {
          ThreadAction(() => Update(Repository.Get<IManga>(), ConfigStorage.Instance.ViewConfig.LibraryFilter.SortDescription))
            .LogException("Автоматическое обновление успешно завершено", "Автоматическое обновление завершено с ошибкой");
        }
      }
    }

    /// <summary>
    /// Обновить мангу.
    /// </summary>
    /// <param name="manga">Обновляемая манга.</param>
    public void Update(IManga manga)
    {
      Update(Enumerable.Repeat(manga, 1), new SortDescription());
    }
    
    /// <summary>
    /// Обновить мангу.
    /// </summary>
    /// <param name="mangas">Обновляемая манга.</param>
    /// <param name="sort">Сортировка.</param>
    public void Update(IEnumerable<IManga> mangas, SortDescription sort)
    {
      OnLibraryChanged(new LibraryViewModelArgs(null, null, MangaOperation.None, LibraryOperation.UpdateStarted));
      Log.Info(Strings.Library_Status_Update);
      try
      {
        mangaIndex = 0;
        mangas = sort.Direction == ListSortDirection.Ascending ?
          mangas.OrderBy(m => m.Name) :
          mangas.OrderByDescending(m => m.Name);
        var listMangas = mangas.Where(m => m.NeedUpdate).ToList();
        mangasCount = listMangas.Count;
        foreach (var current in listMangas)
        {
          DownloadManager.CheckPause().Wait();

          Log.Info(Strings.Library_Status_MangaUpdate + current.Name);
          OnLibraryChanged(new LibraryViewModelArgs(null, current, MangaOperation.UpdateStarted, LibraryOperation.UpdateMangaChanged));
          current.PropertyChanged += CurrentOnDownloadChanged;
          current.Download().Wait();
          current.PropertyChanged -= CurrentOnDownloadChanged;
          if (current.NeedCompress ?? current.Setting.CompressManga)
            current.Compress();
          mangaIndex++;
          if (current.IsDownloaded)
            OnLibraryChanged(new LibraryViewModelArgs(null, current, MangaOperation.UpdateCompleted, LibraryOperation.UpdateMangaChanged));
        }
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
      finally
      {
        OnLibraryChanged(new LibraryViewModelArgs(null, null, MangaOperation.None, LibraryOperation.UpdateCompleted));
        Log.Info(Strings.Library_Status_UpdateComplete);
        ConfigStorage.Instance.AppConfig.LastUpdate = DateTime.Now;
      }
    }

    private void CurrentOnDownloadChanged(object sender, PropertyChangedEventArgs args)
    {
      if (args.PropertyName == nameof(IManga.Downloaded))
      {
        var mangas = (IManga)sender;
        var percent = (double)(100 * mangaIndex + mangas.Downloaded) / (mangasCount * 100);
        OnLibraryChanged(new LibraryViewModelArgs(percent, mangas, MangaOperation.None, LibraryOperation.UpdatePercentChanged));
      }
    }

    #endregion

    public event EventHandler<LibraryViewModelArgs> LibraryChanged; 

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public LibraryViewModel()
    {
      Timer = new Timer(TimerTick, null, new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));
    }

    protected virtual void OnLibraryChanged(LibraryViewModelArgs e)
    {
      LibraryChanged?.Invoke(this, e);
    }
  }

  public struct LibraryViewModelArgs
  {
    public double? Percent { get; }
    public IManga Manga { get; }
    public MangaOperation MangaOperation { get; }
    public LibraryOperation LibraryOperation { get; }

    public LibraryViewModelArgs(double? percent, IManga manga, 
      MangaOperation mangaOperation, LibraryOperation libraryOperation)
    {
      this.Percent = percent;
      this.Manga = manga;
      this.MangaOperation = mangaOperation;
      this.LibraryOperation = libraryOperation;
    }
  }

  public enum MangaOperation
  {
    Added,
    Deleted,
    UpdateStarted,
    UpdateCompleted,
    None
  }

  public enum LibraryOperation
  {
    UpdateStarted,
    UpdatePercentChanged,
    UpdateMangaChanged,
    UpdateCompleted,
  }
}
