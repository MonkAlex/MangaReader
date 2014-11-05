using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Shell;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using MangaReader.Manga;
using MangaReader.Properties;

namespace MangaReader.Services
{
  public class Library
  {
    /// <summary>
    /// Ссылка на файл базы.
    /// </summary>
    private static readonly string DatabaseFile = Settings.WorkFolder + @".\db";

    /// <summary>
    /// База манги.
    /// </summary>
    private static List<string> Database = Serializer<List<string>>.Load(DatabaseFile);

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
      foreach (var manga in Cache.Get())
      {
        DatabaseMangas.Add(manga);
      }
      DatabaseMangas.CollectionChanged += (s, e) => Cache.Add(DatabaseMangas);
      formDispatcher = main.Dispatcher;
      taskBar = main.TaskBar;
      taskbarIcon = main.NotifyIcon;
      main.FormLibrary.ItemsSource = DatabaseMangas;
    }

    /// <summary>
    /// Добавить мангу.
    /// </summary>
    /// <param name="url"></param>
    public static void Add(string url)
    {
      if (Database.Contains(url))
        return;

      var newManga = Mangas.Create(url);
      if (!newManga.IsValid)
        return;

      Database.Add(url);
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

      Database.Remove(manga.Url);
      formDispatcher.Invoke(() => DatabaseMangas.Remove(manga));
      Status = Strings.Library_Status_MangaRemoved + manga.Name;
    }

    /// <summary>
    /// Сохранить библиотеку.
    /// </summary>
    public static void Save()
    {
      var sortedDatabase = Database
          .OrderBy(r =>
          {
            var index = DatabaseMangas
                .IndexOf(DatabaseMangas
                    .FirstOrDefault(m => m.Url == r));
            return index < 0 ? int.MaxValue : index;

          }).ToList();

      Serializer<List<string>>.Save(DatabaseFile, sortedDatabase);
    }

    /// <summary>
    /// Сконвертировать в новый формат.
    /// </summary>
    public static void Convert()
    {
      if (Database != null)
        return;

      Database = File.Exists(DatabaseFile) ? new List<string>(File.ReadAllLines(DatabaseFile)) : new List<string>();
      Save();
    }
    /// <summary>
    /// Получить мангу в базе.
    /// </summary>
    /// <returns>Манга.</returns>
    public static ObservableCollection<Mangas> GetMangas()
    {
      Parallel.ForEach(Database, UpdateMangaByUrl);
      return DatabaseMangas;
    }

    /// <summary>
    /// Обновить состояние манги в библиотеке.
    /// </summary>
    /// <param name="line">Ссылка на мангу.</param>
    private static void UpdateMangaByUrl(string line)
    {
      Mangas manga = null;
      if (DatabaseMangas != null)
        lock (DispatcherLock)
          manga = DatabaseMangas.FirstOrDefault(m => m.Url == line);

      if (manga == null)
      {
        var newManga = Mangas.Create(line);
        lock (DispatcherLock)
          formDispatcher.Invoke(() => DatabaseMangas.Add(newManga));

      }
      else
      {
        var index = 0;
        lock (DispatcherLock)
          index = DatabaseMangas.IndexOf(manga);
        manga.Refresh();
        lock (DispatcherLock)
          formDispatcher.Invoke(() =>
          {
            DatabaseMangas.RemoveAt(index);
            DatabaseMangas.Insert(index, manga);
          });
      }
    }

    /// <summary>
    /// Обновить мангу.
    /// </summary>
    /// <param name="manga">Обновляемая манга. По умолчанию - вся.</param>
    public static void Update(Mangas manga = null)
    {
      Library.SetTaskbarState(0, TaskbarItemProgressState.Indeterminate);

      ObservableCollection<Mangas> mangas;
      if (manga != null)
      {
        UpdateMangaByUrl(manga.Url);
        mangas = new ObservableCollection<Mangas> { manga };
      }
      else
      {
        Status = Strings.Library_Status_Update;
        mangas = GetMangas();
      }

      try
      {
        Library.SetTaskbarState(0, TaskbarItemProgressState.Normal);
        var mangaIndex = 0;
        foreach (var current in mangas)
        {
          Status = Strings.Library_Status_MangaUpdate + current.Name;
          current.DownloadProgressChanged += (sender, args) =>
              Library.SetTaskbarState((double)(100 * mangaIndex + args.Downloaded) / (mangas.Count * 100));
          current.Download();
          if (Settings.CompressManga)
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


    public Library()
    {
      throw new Exception("Use methods.");
    }
  }
}
