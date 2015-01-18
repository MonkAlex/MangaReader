using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Shell;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using MangaReader.Manga;
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
    /// Манга в библиотеке.
    /// </summary>
    public static ObservableCollection<Mangas> DatabaseMangas = new ObservableCollection<Mangas>(Enumerable.Empty<Mangas>());

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
    /// Показать сообщение в трее.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="context">Контекст.</param>
    internal static void ShowInTray(string message, object context)
    {
      if (Settings.MinimizeToTray)
        lock (TaskbarIconLock)
          lock (DispatcherLock)
            formDispatcher.Invoke(() =>
            {
              taskbarIcon.ShowBalloonTip(Strings.Title, message, BalloonIcon.Info);
              taskbarIcon.DataContext = context;
            });

    }

    /// <summary>
    /// Установить состояние программы на панели задач.
    /// </summary>
    /// <param name="percent">Процент.</param>
    /// <param name="state">Состояние.</param>
    internal static void SetTaskbarState(double? percent = null, TaskbarItemProgressState? state = null)
    {
      lock (TaskbarLock)
        lock (DispatcherLock)
          formDispatcher.Invoke(() =>
          {
            if (state.HasValue)
              taskBar.ProgressState = state.Value;
            if (percent.HasValue)
              taskBar.ProgressValue = percent.Value;
          });
    }

    #region Методы

    /// <summary>
    /// Инициализация библиотеки - заполнение массива из кеша.
    /// </summary>
    /// <returns></returns>
    public static void Initialize(Table main)
    {
      DatabaseMangas = Cache.Get();
      DatabaseMangas.CollectionChanged += (s, e) => { Cache.Add(DatabaseMangas); FilterChanged(main); };
      formDispatcher = main.Dispatcher;
      taskBar = main.TaskBar;
      taskbarIcon = main.NotifyIcon;
      main.Type.Items.Add("Все");
      main.Type.Items.Add("AdultManga");
      main.Type.Items.Add("AComics");
      main.Type.Items.Add("ReadManga");
      main.Type.SelectionChanged += (s, a) => FilterChanged(main);
      main.NameFilter.TextChanged += (s, a) => FilterChanged(main);
      main.Uncompleted.Click += (s, a) => FilterChanged(main);
      main.OnlyUpdate.Click += (s, a) => FilterChanged(main);
      main.Type.SelectedIndex = 0;
    }

    private static void FilterChanged(Table main)
    {
      var query = DatabaseMangas.Where(n => n != null);
      if (main.Type.SelectedItem.ToString() != "Все")
        query = query.Where(n => n.Uri.OriginalString.ToLowerInvariant().Contains(main.Type.SelectedItem.ToString().ToLowerInvariant()));
      if (main.Uncompleted.IsChecked == true)
        query = query.Where(n => n.IsCompleted != "завершен");
      if (main.OnlyUpdate.IsChecked == true)
        query = query.Where(n => n.NeedUpdate);
      if (main.NameFilter.Text.Any())
        query = query.Where(n => n.Name.ToLowerInvariant().Contains(main.NameFilter.Text.ToLowerInvariant()));
      main.FormLibrary.ItemsSource = query;
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

      formDispatcher.Invoke(() => DatabaseMangas.Add(newManga));
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

      Log.Add(Strings.Manga_Action_Remove + manga.Name);

      formDispatcher.Invoke(() => DatabaseMangas.Remove(manga));
      manga.Delete();

      Status = Strings.Library_Status_MangaRemoved + manga.Name;
      Log.Add(Strings.Library_Status_MangaRemoved + manga.Name);
    }

    /// <summary>
    /// Сконвертировать в новый формат.
    /// </summary>
    internal static void Convert(ConverterProcess process)
    {
      if (!File.Exists(DatabaseFile))
        return;

      var database = Serializer<List<string>>.Load(DatabaseFile) ?? new List<string>(File.ReadAllLines(DatabaseFile));

      if (process != null && database.Any())
        process.IsIndeterminate = false;

      var mangaUrls = Mapping.Environment.Session.Query<Mangas>().Select(m => m.Uri.OriginalString).ToList();

      foreach (var dbstring in database)
      {
        if (process != null)
          process.Percent += 100.0 / database.Count;
        if (!mangaUrls.Contains(dbstring))
          Mangas.Create(dbstring);
      }

      BackupFile.MoveToBackup(DatabaseFile);
    }

    /// <summary>
    /// Обновить мангу.
    /// </summary>
    /// <param name="manga">Обновляемая манга. По умолчанию - вся.</param>
    public static void Update(Mangas manga = null)
    {
      Library.SetTaskbarState(0, TaskbarItemProgressState.Indeterminate);

      List<Mangas> mangas;
      if (manga != null)
      {
        mangas = new List<Mangas> { manga };
      }
      else
      {
        Status = Strings.Library_Status_Update;
        mangas = Mapping.Environment.Session.Query<Mangas>().Where(m => m.NeedUpdate).ToList();
      }

      try
      {
        Library.SetTaskbarState(0, TaskbarItemProgressState.Normal);
        var mangaIndex = 0;
        mangas = mangas.Where(m => m.NeedUpdate).ToList();
        foreach (var current in mangas)
        {
          Status = Strings.Library_Status_MangaUpdate + current.Name;
          current.DownloadProgressChanged += (sender, args) =>
              Library.SetTaskbarState((double)(100 * mangaIndex + args.Downloaded) / (mangas.Count * 100));
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
        if (manga == null)
          Settings.LastUpdate = DateTime.Now;
      }

    }

    #endregion

  }
}
