using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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
    private readonly Config.Config config;
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
    /// <param name="task">Выполняемое действие.</param>
    /// <remarks>Только одно действие за раз. Доступность выполнения можно проверить в IsAvaible.</remarks>
    public async Task ThreadAction(Task task)
    {
      if (!IsAvaible)
        throw new MangaReaderException("Library not avaible.");

      try
      {
        IsAvaible = false;
        await task.ConfigureAwait(false);
      }
      finally
      {
        IsAvaible = true;
      }
    }

    #region Методы

    /// <summary>
    /// Добавить мангу.
    /// </summary>
    /// <param name="uri">Ссылка на мангу.</param>
    public async Task<(bool Success, IManga Manga)> Add(Uri uri)
    {
      using (var context = Repository.GetEntityContext())
      {
        var manga = await context.Get<IManga>().FirstOrDefaultAsync(m => m.Uri == uri).ConfigureAwait(false);
        if (manga != null)
        {
          Log.Info("Манга уже добавлена.");
          return (false, manga);
        }
      }

      var newManga = await Mangas.CreateFromWeb(uri).ConfigureAwait(false);
      if (newManga == null || !await newManga.IsValid().ConfigureAwait(false))
      {
        Log.Info("Не удалось найти мангу.");
        return (false, null);
      }

      OnLibraryChanged(new LibraryViewModelArgs(null, newManga, MangaOperation.Added, LibraryOperation.UpdateMangaChanged));
      Log.Info(Strings.Library_Status_MangaAdded + newManga.Name);
      return (true, newManga);
    }

    /// <summary>
    /// Удалить мангу.
    /// </summary>
    /// <param name="manga"></param>
    public async Task Remove(IManga manga)
    {
      if (manga == null)
        return;

      OnLibraryChanged(new LibraryViewModelArgs(null, manga, MangaOperation.Deleted, LibraryOperation.UpdateMangaChanged));
      try
      {
        using (var context = Repository.GetEntityContext($"Remove manga {manga.Name}"))
          await context.Delete(manga).ConfigureAwait(false);
        Log.Info(Strings.Library_Status_MangaRemoved + manga.Name);
      }
      catch (System.Exception e)
      {
        Log.Exception(e);
        OnLibraryChanged(new LibraryViewModelArgs(null, manga, MangaOperation.Added, LibraryOperation.UpdateMangaChanged));
      }
    }

    private void TimerTick(object sender)
    {
      if (IsAvaible && config.AppConfig.AutoUpdateInHours > 0 &&
          DateTime.Now > config.AppConfig.LastUpdate.AddHours(config.AppConfig.AutoUpdateInHours))
      {
        Log.InfoFormat("{0} Время последнего обновления - {1}, частота обновления - каждые {2} часов.",
          Strings.AutoUpdate, config.AppConfig.LastUpdate, config.AppConfig.AutoUpdateInHours);

        if (IsAvaible)
        {
          ThreadAction(Update()).LogException("Автоматическое обновление успешно завершено", "Автоматическое обновление завершено с ошибкой");
        }
      }
    }

    /// <summary>
    /// Обновить мангу.
    /// </summary>
    public Task Update()
    {
      return Update((List<int>)null);
    }

    /// <summary>
    /// Обновить мангу.
    /// </summary>
    /// <param name="mangas">Обновляемая манга.</param>
    public Task Update(List<int> mangas)
    {
      var saved = config.ViewConfig.LibraryFilter.SortDescription;
      switch (saved.PropertyName)
      {
        case nameof(IManga.Created):
          Expression<Func<IManga, DateTime?>> createdSelector = m => m.Created;
          return Update(mangas, c => saved.Direction == ListSortDirection.Ascending ? c.OrderBy(createdSelector) : c.OrderByDescending(createdSelector));
        case nameof(IManga.DownloadedAt):
          Expression<Func<IManga, DateTime?>> downloadSelector = m => m.DownloadedAt;
          return Update(mangas, c => saved.Direction == ListSortDirection.Ascending ? c.OrderBy(downloadSelector) : c.OrderByDescending(downloadSelector));
        default:
          Expression<Func<IManga, string>> nameSelector = m => m.Name;
          return Update(mangas, c => saved.Direction == ListSortDirection.Ascending ? c.OrderBy(nameSelector) : c.OrderByDescending(nameSelector));
      }
    }

    /// <summary>
    /// Обновить мангу.
    /// </summary>
    /// <param name="mangas">Обновляемая манга.</param>
    /// <param name="orderBy">Сортировка.</param>
    public async Task Update(List<int> mangas, Func<IQueryable<IManga>, IOrderedQueryable<IManga>> orderBy)
    {
      OnLibraryChanged(new LibraryViewModelArgs(null, null, MangaOperation.None, LibraryOperation.UpdateStarted));
      Log.Info(Strings.Library_Status_Update);
      try
      {
        mangaIndex = 0;
        List<int> materialized;
        if (mangas != null)
        {
          materialized = mangas;
        }
        else
        {
          using (var context = Repository.GetEntityContext("Prepare selected manga for update"))
          {
            var entities = context.Get<IManga>().Where(m => m.NeedUpdate);

            // Пытаемся качать в порядке сортировки.
            if (orderBy != null)
              entities = orderBy(entities);

            materialized = await entities.Select(e => e.Id).ToListAsync().ConfigureAwait(false);
          }
        }

        mangasCount = materialized.Count;
        
        void OnMangaAddedWhenLibraryUpdating(object sender, LibraryViewModelArgs args)
        {
          if (args.MangaOperation == MangaOperation.Added)
          {
            materialized.Add(args.Manga.Id);
            mangasCount = materialized.Count;
            Log.Info($"Манга {args.Manga.Name} добавлена при обновлении и будет загружена автоматически");
          }
        }

        LibraryChanged += OnMangaAddedWhenLibraryUpdating;
        for (var i = 0; i < materialized.Count; i++)
        {
          var mangaId = materialized[i];
          await DownloadManager.CheckPause().ConfigureAwait(false);

          using (var context = Repository.GetEntityContext($"Download updates for manga with id {mangaId}"))
          {
            var current = await context.Get<IManga>().SingleOrDefaultAsync(m => m.Id == mangaId).ConfigureAwait(false);
            if (current == null || !current.NeedUpdate)
            {
              Log.Add(current == null ?
                $"Manga with id {mangaId} not found" :
                $"Manga with id {mangaId} has disabled updates");
              continue;
            }

            Log.Info(Strings.Library_Status_MangaUpdate + current.Name);
            OnLibraryChanged(new LibraryViewModelArgs(null, current, MangaOperation.UpdateStarted, LibraryOperation.UpdateMangaChanged));
            current.PropertyChanged += CurrentOnDownloadChanged;
            UpdateDownloadPercent(current);
            await current.Download().ConfigureAwait(false);
            current.PropertyChanged -= CurrentOnDownloadChanged;
            if (current.NeedCompress ?? current.Setting.CompressManga)
              current.Compress();
            mangaIndex++;
            if (current.IsDownloaded)
              OnLibraryChanged(new LibraryViewModelArgs(null, current, MangaOperation.UpdateCompleted, LibraryOperation.UpdateMangaChanged));
          }
        }

        LibraryChanged -= OnMangaAddedWhenLibraryUpdating;
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
      finally
      {
        OnLibraryChanged(new LibraryViewModelArgs(null, null, MangaOperation.None, LibraryOperation.UpdateCompleted));
        Log.Info(Strings.Library_Status_UpdateComplete);
        config.AppConfig.LastUpdate = DateTime.Now;
      }
    }

    private void CurrentOnDownloadChanged(object sender, PropertyChangedEventArgs args)
    {
      if (args.PropertyName == nameof(IManga.Downloaded))
      {
        var mangas = (IManga)sender;
        UpdateDownloadPercent(mangas);
      }
    }

    private void UpdateDownloadPercent(IManga mangas)
    {
      var percent = (double)(100 * mangaIndex + mangas.Downloaded) / (mangasCount * 100);
      OnLibraryChanged(new LibraryViewModelArgs(percent, mangas, MangaOperation.None, LibraryOperation.UpdatePercentChanged));
    }

    #endregion

    public event EventHandler<LibraryViewModelArgs> LibraryChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public LibraryViewModel(Config.Config config)
    {
      Timer = new Timer(TimerTick, null, new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));
      this.config = config;
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

    public override string ToString()
    {
      return $"{LibraryOperation}-{MangaOperation}-{Manga?.Name}-{Percent}";
    }

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
