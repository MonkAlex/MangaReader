using System;
using System.Collections.Generic;
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
using NHibernate.Linq;

namespace MangaReader.Services
{
  public static class Library
  {
    /// <summary>
    /// Ссылка на файл базы.
    /// </summary>
    private static readonly string DatabaseFile = Settings.WorkFolder + @".\db";

    /// <summary>
    /// Статус библиотеки.
    /// </summary>
    public static string Status = string.Empty;

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

    /// <summary>
    /// Показать сообщение в трее.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="context">Контекст.</param>
    internal static void ShowInTray(string message, object context)
    {
      if (Settings.MinimizeToTray)
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
    public static void Initialize(Table main)
    {
      formDispatcher = main.Dispatcher;
      taskBar = main.TaskBar;
      taskbarIcon = main.NotifyIcon;
      main.Type.Items.Add(Strings.Library_TypeFilter_All);
      main.Type.Items.Add("AdultManga");
      main.Type.Items.Add("AComics");
      main.Type.Items.Add("ReadManga");
      main.Type.SelectionChanged += (s, a) => FilterChanged(main);
      main.NameFilter.TextChanged += (s, a) => FilterChanged(main);
      main.Uncompleted.Click += (s, a) => FilterChanged(main);
      main.OnlyUpdate.Click += (s, a) => FilterChanged(main);
      main.Type.SelectedIndex = 0;
    }

    public static void FilterChanged(Table main)
    {
      var query = Mapping.Environment.Session.Query<Mangas>().Where(n => n != null);
      if (main.Type.SelectedItem.ToString() != Strings.Library_TypeFilter_All)
        query = query.Where(n => n.Uri.ToString().ToUpper().Contains(main.Type.SelectedItem.ToString().ToUpper()));
      if (main.Uncompleted.IsChecked == true)
        query = query.Where(n => !n.IsCompleted);
      if (main.OnlyUpdate.IsChecked == true)
        query = query.Where(n => n.NeedUpdate);
      if (main.NameFilter.Text.Any())
        query = query.Where(n => (n.IsNameChanged ? n.LocalName : n.ServerName).ToLowerInvariant().
          Contains(main.NameFilter.Text.ToLowerInvariant()));
      main.FormLibrary.ItemsSource = query.ToList().OrderBy(m => m.Name);
    }

    /// <summary>
    /// Добавить мангу.
    /// </summary>
    /// <param name="url"></param>
    public static void Add(string url)
    {
      Uri uri;
      if (Uri.TryCreate(url, UriKind.Absolute, out uri))
        Add(uri);
      else
        Library.Status = "Некорректная ссылка.";
    }

    /// <summary>
    /// Добавить мангу.
    /// </summary>
    /// <param name="uri"></param>
    public static void Add(Uri uri)
    {
      if (Mapping.Environment.Session.Query<Mangas>().Any(m => m.Uri == uri))
        return;

      var newManga = Mangas.Create(uri);
      if (!newManga.IsValid())
        return;

      Status = Strings.Library_Status_MangaAdded + newManga.Name;
    }

    /// <summary>
    /// Удалить мангу.
    /// </summary>
    /// <param name="manga"></param>
    public static void Remove(Mangas manga)
    {
      if (manga == null)
        return;

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
      if (process.Version.CompareTo(Settings.DatabaseVersion) > 0)
      {
        process.Percent = 0;
        var acomics = Mapping.Environment.Session.Query<Acomics>().ToList();
        if (acomics.Any())
          process.IsIndeterminate = false;

        foreach (var acomic in acomics)
        {
          process.Percent += 100.0 / acomics.Count;
          Getter.UpdateContentType(acomic);
        }
      }
    }

    private static void ConvertBaseTo24(ConverterProcess process)
    {
      var database = Serializer<List<string>>.Load(DatabaseFile) ?? new List<string>(File.ReadAllLines(DatabaseFile));

      if (process != null && database.Any())
        process.IsIndeterminate = false;

      var mangaUrls = Mapping.Environment.Session.Query<Mangas>().Select(m => m.Uri.ToString()).ToList();

      foreach (var dbstring in database)
      {
        if (process != null)
          process.Percent += 100.0 / database.Count;
        if (!mangaUrls.Contains(dbstring))
          Mangas.Create(dbstring);
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
          if (current.NeedCompress ?? Settings.CompressManga)
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
        Settings.LastUpdate = DateTime.Now;
      }

    }

    #endregion

  }
}
