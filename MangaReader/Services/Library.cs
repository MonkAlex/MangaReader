using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Shell;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using MangaReader.Manga;
using MangaReader.Manga.Acomic;
using MangaReader.Properties;
using MangaReader.Services.Config;
using MangaReader.UI.MainForm;
using NHibernate.Linq;
using Environment = MangaReader.Mapping.Environment;

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
    /// Служба управления UI главного окна.
    /// </summary>
    private static Dispatcher formDispatcher;

    private static readonly object DispatcherLock = new object();

    /// <summary>
    /// Таскбар окна.
    /// </summary>
    private static TaskbarItemInfo taskBar;

    private static readonly object TaskbarLock = new object();

    /// <summary>
    /// Иконка в трее.
    /// </summary>
    private static TaskbarIcon taskbarIcon;

    private static readonly object TaskbarIconLock = new object();

    /// <summary>
    /// Признак паузы.
    /// </summary>
    public static bool IsPaused { get; set; }

    public static ObservableCollection<Mangas> LibraryMangas
    {
      get
      {
        return _libraryMangas ?? (_libraryMangas = new ObservableCollection<Mangas>(Environment.Session.Query<Mangas>().Where(n => n != null)));
      }
    }

    private static ObservableCollection<Mangas> _libraryMangas;

    public static Mangas SelectedManga { get; set; }

    private static Thread _loadThread;

    /// <summary>
    /// Библиотека доступна, т.е. не в процессе обновления.
    /// </summary>
    internal static bool IsAvaible { get { return _loadThread == null || _loadThread.ThreadState == ThreadState.Stopped; } }

    /// <summary>
    /// Показать сообщение в трее.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="context">Контекст.</param>
    internal static void ShowInTray(string message, object context)
    {
      if (ConfigStorage.Instance.AppConfig.MinimizeToTray)
        using (TimedLock.Lock(TaskbarIconLock))
        {
          using (TimedLock.Lock(DispatcherLock))
          {
            formDispatcher.Invoke(() =>
            {
              taskbarIcon.ShowBalloonTip(Strings.Title, message, BalloonIcon.Info);
              taskbarIcon.DataContext = context;
            });
          }
        }
    }

    /// <summary>
    /// Выполнить тяжелое действие изменения библиотеки в отдельном потоке.
    /// </summary>
    /// <param name="action">Выполняемое действие.</param>
    /// <remarks>Только одно действие за раз. Доступность выполнения можно проверить в IsAvaible.</remarks>
    internal static void ThreadAction(ThreadStart action)
    {
      if (_loadThread == null || _loadThread.ThreadState == ThreadState.Stopped)
        _loadThread = new Thread(action);
      if (_loadThread.ThreadState == ThreadState.Unstarted)
        _loadThread.Start();
    }

    /// <summary>
    /// Установить состояние программы на панели задач.
    /// </summary>
    /// <param name="percent">Процент.</param>
    /// <param name="state">Состояние.</param>
    internal static void SetTaskbarState(double? percent = null, TaskbarItemProgressState? state = null)
    {
      using (TimedLock.Lock(TaskbarLock))
      {
        using (TimedLock.Lock(DispatcherLock))
        {
          formDispatcher.Invoke(() =>
          {
            if (state.HasValue)
              taskBar.ProgressState = state.Value;
            if (percent.HasValue)
              taskBar.ProgressValue = percent.Value;
          });
        }
      }
    }

    #region Методы

    /// <summary>
    /// Инициализация библиотеки - заполнение массива из кеша.
    /// </summary>
    /// <returns></returns>
    public static void Initialize(BaseForm main)
    {
      formDispatcher = main.Dispatcher;
      taskBar = main.TaskbarItemInfo;
      taskbarIcon = main.NotifyIcon;
      SelectedManga = main.View.Cast<Mangas>().FirstOrDefault();
    }

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
      if (Environment.Session.Query<Mangas>().Any(m => m.Uri == uri))
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

      Status = Strings.Library_Status_MangaRemoved + manga.Name;
      Log.Add(Strings.Library_Status_MangaRemoved + manga.Name);
    }

    /// <summary>
    /// Сконвертировать в новый формат.
    /// </summary>
    internal static void Convert(ConverterProcess process)
    {
      if (File.Exists(DatabaseFile))
        ConvertBaseTo24(process);

      Convert24To27(process);
    }

    private static void Convert24To27(ConverterProcess process)
    {
      var version = new Version(1, 27, 5584);
      if (version.CompareTo(ConfigStorage.Instance.DatabaseConfig.Version) > 0 && process.Version.CompareTo(version) >= 0)
      {
        process.Percent = 0;
        using (var session = Environment.OpenSession())
        {
          var acomics = session.Query<Acomics>().ToList();
          if (acomics.Any())
            process.IsIndeterminate = false;

          using (var tranc = session.BeginTransaction())
          {
            foreach (var acomic in acomics)
            {
              process.Percent += 100.0 / acomics.Count;
              Getter.UpdateContentType(acomic);
              acomic.Save(session, tranc);
            }
            tranc.Commit();
          }
        }
      }
    }

    private static void ConvertBaseTo24(ConverterProcess process)
    {
      var database = Serializer<List<string>>.Load(DatabaseFile) ?? new List<string>(File.ReadAllLines(DatabaseFile));

      if (process != null && database.Any())
        process.IsIndeterminate = false;

      using (var session = Environment.OpenSession())
      {
        var mangaUrls = session.Query<Mangas>().Select(m => m.Uri.ToString()).ToList();

        foreach (var dbstring in database)
        {
          if (process != null)
            process.Percent += 100.0/database.Count;
          if (!mangaUrls.Contains(dbstring))
#pragma warning disable CS0612 // Obsolete методы используются для конвертации
            Mangas.Create(dbstring);
#pragma warning restore CS0612
        }
      }

      Backup.MoveToBackup(DatabaseFile);
    }

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
      Library.SetTaskbarState(0, TaskbarItemProgressState.Indeterminate);
      Status = Strings.Library_Status_Update;
      try
      {
        Library.SetTaskbarState(0, TaskbarItemProgressState.Normal);
        var mangaIndex = 0;
        mangas = sort.Direction == ListSortDirection.Ascending ?
          mangas.OrderBy(m => m.Name) :
          mangas.OrderByDescending(m => m.Name);
        var listMangas = mangas.Where(m => m.NeedUpdate).ToList();
        foreach (var current in listMangas)
        {
          CheckPause();

          Status = Strings.Library_Status_MangaUpdate + current.Name;
          current.DownloadProgressChanged += (sender, args) =>
              Library.SetTaskbarState((double)(100 * mangaIndex + args.Downloaded) / (listMangas.Count * 100));
          current.Download();
          if (current.NeedCompress ?? current.Setting.CompressManga)
            current.Compress();
          mangaIndex++;
          if (current.IsDownloaded)
            Library.ShowInTray(Strings.Library_Status_MangaUpdate + current.Name + " завершено.", current);
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
        Library.SetTaskbarState(0, TaskbarItemProgressState.None);
        Status = Strings.Library_Status_UpdateComplete;
        ConfigStorage.Instance.AppConfig.LastUpdate = DateTime.Now;
      }
    }

    #endregion

  }
}
