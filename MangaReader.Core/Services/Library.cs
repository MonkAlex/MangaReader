using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Properties;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{
  public static class Library
  {
    /// <summary>
    /// Таймер для автообновления манги.
    /// </summary>
    private static readonly Timer Timer = new Timer(TimerTick, null, new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));

    /// <summary>
    /// Статус библиотеки.
    /// </summary>
    public static string Status
    {
      get { return status; }
      set
      {
        status = value;
        OnStatusChanged(value);
      }
    }

    /// <summary>
    /// Признак паузы.
    /// </summary>
    public static bool IsPaused
    {
      get { return isPaused; }
      set
      {
        isPaused = value;
        OnPauseChanged(value);
      }
    }

    private static int mangaIndex;

    private static int mangasCount;
    private static bool isAvaible = true;
    private static bool isPaused;
    private static string status;

    /// <summary>
    /// Библиотека доступна, т.е. не в процессе обновления.
    /// </summary>
    public static bool IsAvaible
    {
      get { return isAvaible; }
      private set
      {
        isAvaible = value;
        OnAvaibleChanged(value);
      }
    }

    /// <summary>
    /// Выполнить тяжелое действие изменения библиотеки в отдельном потоке.
    /// </summary>
    /// <param name="action">Выполняемое действие.</param>
    /// <remarks>Только одно действие за раз. Доступность выполнения можно проверить в IsAvaible.</remarks>
    public async static void ThreadAction(Action action)
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
    public static bool Add(string url)
    {
      Uri uri;
      if (Uri.TryCreate(url, UriKind.Absolute, out uri) && Add(uri))
        return true;

      Library.Status = "Некорректная ссылка.";
      return false;
    }

    /// <summary>
    /// Добавить мангу.
    /// </summary>
    /// <param name="uri"></param>
    public static bool Add(Uri uri)
    {
      if (Repository.Get<IManga>().Any(m => m.Uri == uri))
        return false;

      var newManga = Mangas.CreateFromWeb(uri);
      if (newManga == null || !newManga.IsValid())
        return false;

      OnMangaAdded(newManga);
      Status = Strings.Library_Status_MangaAdded + newManga.Name;
      return true;
    }

    /// <summary>
    /// Удалить мангу.
    /// </summary>
    /// <param name="manga"></param>
    public static void Remove(IManga manga)
    {
      if (manga == null)
        return;

      manga.Delete();
      OnMangaDeleted(manga);

      var removed = Strings.Library_Status_MangaRemoved + manga.Name;
      Status = removed;
      Log.Add(removed);
    }

    private static void TimerTick(object sender)
    {
      if (IsAvaible && ConfigStorage.Instance.AppConfig.AutoUpdateInHours > 0 &&
        DateTime.Now > ConfigStorage.Instance.AppConfig.LastUpdate.AddHours(ConfigStorage.Instance.AppConfig.AutoUpdateInHours))
      {
        Log.AddFormat("{0} Время последнего обновления - {1}, частота обновления - каждые {2} часов.",
          Strings.AutoUpdate, ConfigStorage.Instance.AppConfig.LastUpdate, ConfigStorage.Instance.AppConfig.AutoUpdateInHours);

        if (IsAvaible)
        {
          ThreadAction(() => Update(Repository.Get<IManga>(), ConfigStorage.Instance.ViewConfig.LibraryFilter.SortDescription));
        }
      }
    }

    /// <summary>
    /// Обновить мангу.
    /// </summary>
    /// <param name="manga">Обновляемая манга.</param>
    public static void Update(IManga manga)
    {
      Update(Enumerable.Repeat(manga, 1), new SortDescription());
    }

    /// <summary>
    /// Реакция на паузу.
    /// </summary>
    public static void CheckPause()
    {
      while (IsPaused)
      {
        Thread.Sleep(1000);
      }
    }

    /// <summary>
    /// Обновить мангу.
    /// </summary>
    /// <param name="mangas">Обновляемая манга.</param>
    /// <param name="sort">Сортировка.</param>
    public static void Update(IEnumerable<IManga> mangas, SortDescription sort)
    {
      OnUpdateStarted();
      Status = Strings.Library_Status_Update;
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
          CheckPause();

          Status = Strings.Library_Status_MangaUpdate + current.Name;
          OnUpdateMangaStarted(current);
          current.DownloadProgressChanged += CurrentOnDownloadProgressChanged;
          current.Download().Wait();
          current.DownloadProgressChanged -= CurrentOnDownloadProgressChanged;
          if (current.NeedCompress ?? current.Setting.CompressManga)
            current.Compress();
          mangaIndex++;
          if (current.IsDownloaded)
            OnUpdateMangaCompleted(current);
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
        OnUpdateCompleted();
        Status = Strings.Library_Status_UpdateComplete;
        ConfigStorage.Instance.AppConfig.LastUpdate = DateTime.Now;
      }
    }

    private static void CurrentOnDownloadProgressChanged(object sender, IManga mangas)
    {
      var percent = (double) (100*mangaIndex + mangas.Downloaded)/(mangasCount*100);
      OnUpdatePercentChanged(percent);
    }

    #endregion

    #region Events

    public static event EventHandler UpdateStarted;

    public static event EventHandler UpdateCompleted;

    public static event EventHandler<double> UpdatePercentChanged;

    public static event EventHandler<IManga> UpdateMangaStarted;

    public static event EventHandler<IManga> UpdateMangaCompleted;

    public static event EventHandler<bool> AvaibleChanged;

    public static event EventHandler<bool> PauseChanged;

    public static event EventHandler<string> StatusChanged;

    public static event EventHandler<IManga> MangaAdded;

    public static event EventHandler<IManga> MangaDeleted;

    private static void OnUpdateStarted()
    {
      UpdateStarted?.Invoke(null, EventArgs.Empty);
    }

    private static void OnUpdateCompleted()
    {
      UpdateCompleted?.Invoke(null, EventArgs.Empty);
    }

    private static void OnUpdatePercentChanged(double e)
    {
      UpdatePercentChanged?.Invoke(null, e);
    }

    private static void OnUpdateMangaCompleted(IManga e)
    {
      UpdateMangaCompleted?.Invoke(null, e);
    }

    private static void OnAvaibleChanged(bool e)
    {
      AvaibleChanged?.Invoke(null, e);
    }

    private static void OnPauseChanged(bool e)
    {
      PauseChanged?.Invoke(null, e);
    }

    private static void OnStatusChanged(string e)
    {
      StatusChanged?.Invoke(null, e);
    }

    private static void OnMangaAdded(IManga e)
    {
      MangaAdded?.Invoke(null, e);
    }

    private static void OnMangaDeleted(IManga e)
    {
      MangaDeleted?.Invoke(null, e);
    }

    private static void OnUpdateMangaStarted(IManga e)
    {
      UpdateMangaStarted?.Invoke(null, e);
    }

    #endregion
  }
}
