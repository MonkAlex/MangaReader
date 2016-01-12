using System;
using System.Linq;
using System.Windows.Shell;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services.Config;
using MangaReader.UI.MainForm;

namespace MangaReader.Services
{
  public static class LibraryWPF
  {
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

    /// <summary>
    /// Инициализация библиотеки - заполнение массива из кеша.
    /// </summary>
    /// <returns></returns>
    public static void Initialize(BaseForm main)
    {
      formDispatcher = main.Dispatcher;
      taskBar = main.TaskbarItemInfo;
      taskbarIcon = main.NotifyIcon;
      Library.SelectedManga = main.View.Cast<Mangas>().FirstOrDefault();
      Library.UpdateStarted += LibraryOnUpdateStarted;
      Library.UpdateCompleted += LibraryOnUpdateCompleted;
      Library.UpdateMangaCompleted += LibraryOnUpdateMangaCompleted;
      Library.UpdatePercentChanged += LibraryOnUpdatePercentChanged;
    }

    private static void LibraryOnUpdatePercentChanged(object sender, double i)
    {
      SetTaskbarState(i);
    }

    private static void LibraryOnUpdateMangaCompleted(object sender, Mangas mangas)
    {
      ShowInTray(Strings.Library_Status_MangaUpdate + mangas.Name + " завершено.", mangas);
    }

    private static void LibraryOnUpdateCompleted(object sender, EventArgs eventArgs)
    {
      SetTaskbarState(0, TaskbarItemProgressState.None);
    }

    private static void LibraryOnUpdateStarted(object sender, EventArgs eventArgs)
    {
      // SetTaskbarState(0, TaskbarItemProgressState.Indeterminate);
      SetTaskbarState(0, TaskbarItemProgressState.Normal);
    }
  }
}