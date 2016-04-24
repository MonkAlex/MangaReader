using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaReader.Core.Exception;
using MangaReader.Core.NHibernate;
using MangaReader.Manga;
using MangaReader.Manga.Acomic;
using MangaReader.Core.Properties;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using NHibernate.Linq;

namespace MangaReader.Services
{
  public static class Library
  {
    /// <summary>
    /// Ссылка на файл базы.
    /// </summary>
    private static readonly string DatabaseFile = ConfigStorage.WorkFolder + @".\db";

    /// <summary>
    /// Статус библиотеки.
    /// </summary>
    public static string Status { get; set; }

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

    public static ObservableCollection<Mangas> LibraryMangas
    {
      get
      {
        return libraryMangas ?? (libraryMangas = new ObservableCollection<Mangas>(Repository.Get<Mangas>().Where(n => n != null)));
      }
    }

    private static ObservableCollection<Mangas> libraryMangas;

    public static Mangas SelectedManga { get; set; }

    private static int mangaIndex;

    private static int mangasCount;
    private static bool isAvaible = true;
    private static bool isPaused;

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
      if (Repository.Get<Mangas>().Any(m => m.Uri == uri))
        return false;

      var newManga = Mangas.Create(uri);
      if (newManga == null || !newManga.IsValid())
        return false;

      Status = Strings.Library_Status_MangaAdded + newManga.Name;
      LibraryMangas.Add(newManga);
      return true;
    }

    /// <summary>
    /// Удалить мангу.
    /// </summary>
    /// <param name="manga"></param>
    public static void Remove(Mangas manga)
    {
      if (manga == null)
        return;

      if (LibraryMangas.Contains(manga))
        LibraryMangas.Remove(manga);

      manga.Delete();

      var removed = Strings.Library_Status_MangaRemoved + manga.Name;
      Status = removed;
      Log.Add(removed);
    }

#pragma warning disable CS0612 // Obsolete методы используются для конвертации
    /// <summary>
    /// Сконвертировать в новый формат.
    /// </summary>
    internal static void Convert(IProcess process)
    {
      if (File.Exists(DatabaseFile))
        ConvertBaseTo24(process);

      Convert24To27(process);
    }

    private static void Convert24To27(IProcess process)
    {
      var version = new Version(1, 27, 5584);
      if (version.CompareTo(ConfigStorage.Instance.DatabaseConfig.Version) > 0 && process.Version.CompareTo(version) >= 0)
      {
        process.Percent = 0;
          var acomics = Repository.Get<Acomics>().ToList();

        foreach (var acomic in acomics)
        {
          Getter.UpdateContentType(acomic);
        }
        acomics.SaveAll();
      }
    }

    private static void ConvertBaseTo24(IProcess process)
    {
      var database = Serializer<List<string>>.Load(DatabaseFile) ?? new List<string>(File.ReadAllLines(DatabaseFile));

      if (process != null && database.Any())
        process.ProgressState = ProgressState.Normal;

      List<string> mangaUrls;
      using (var session = Mapping.OpenSession())
      {
        mangaUrls = session.Query<Mangas>().Select(m => m.Uri.ToString()).ToList();
      }

      foreach (var dbstring in database)
      {
        if (process != null)
          process.Percent += 100.0 / database.Count;
        if (!mangaUrls.Contains(dbstring))
          Mangas.Create(dbstring);
      }

      Backup.MoveToBackup(DatabaseFile);
    }
#pragma warning restore CS0612

    /// <summary>
    /// Обновить мангу.
    /// </summary>
    /// <param name="manga">Обновляемая манга.</param>
    public static void Update(Mangas manga)
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
    public static void Update(IEnumerable<Mangas> mangas, SortDescription sort)
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
          current.DownloadProgressChanged += CurrentOnDownloadProgressChanged;
          current.Download();
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
      catch (Exception ex)
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

    private static void CurrentOnDownloadProgressChanged(object sender, Mangas mangas)
    {
      var percent = (double) (100*mangaIndex + mangas.Downloaded)/(mangasCount*100);
      OnUpdatePercentChanged(percent);
    }

    #endregion

    #region Events

    public static event EventHandler UpdateStarted;

    public static event EventHandler UpdateCompleted;

    public static event EventHandler<double> UpdatePercentChanged;

    public static event EventHandler<Mangas> UpdateMangaCompleted;

    public static event EventHandler<bool> AvaibleChanged;

    public static event EventHandler<bool> PauseChanged;

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

    private static void OnUpdateMangaCompleted(Mangas e)
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

    #endregion

  }
}
