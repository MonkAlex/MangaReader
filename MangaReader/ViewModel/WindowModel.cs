using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MangaReader.Core.Convertation;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Properties;
using MangaReader.Services.Config;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class WindowModel : ProcessModel, IDisposable
  {
    private static Lazy<WindowModel> lazyModel = new Lazy<WindowModel>(() => new WindowModel()); 
    public static WindowModel Instance { get { return lazyModel.Value; } }

    private object content;
    private MainPageModel model;
    public ICommand UpdateAll { get; set; }

    public ICommand Close { get; set; }

    public TaskbarIconModel TaskbarIcon { get; set; }

    public object Content
    {
      get { return content; }
      set
      {
        content = value;
        OnPropertyChanged();
      }
    }

    public override void Show()
    {
      base.Show();
      var window = ViewService.Instance.TryGet<System.Windows.Window>(this);
      if (window != null)
      {
        window.StateChanged += (o, a) => WindowOnStateChanged(window, a);
        window.Closing += (o, a) => WindowOnClosing(window, a);

        ConfigStorage.Instance.ViewConfig.UpdateWindowState(window);
        window.Show();
      }
      model = new MainPageModel();
      UpdateAll = new UpdateAllCommand(model.Library);

      model.Library.LibraryChanged += LibraryOnLibraryChanged;
      model.Library.PropertyChanged += LibraryOnPropertyChanged;

      if (window != null)
        TaskbarIcon = new TaskbarIconModel(window.FindName("TaskbarIcon"), model.Library);

      model.Show();

      Client.ClientOnClientBeenClosed(this, EventArgs.Empty);
    }

    private void LibraryOnPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      if (args.PropertyName == nameof(model.Library.IsPaused))
      {
        if (model.Library.IsPaused)
        {
          beforePause = ProgressState;
          ProgressState = ProgressState.Paused;
        }
        else
          ProgressState = beforePause;
      }
    }

    private void LibraryOnLibraryChanged(object sender, LibraryViewModelArgs args)
    {
      switch (args.LibraryOperation)
      {
        case LibraryOperation.UpdateStarted:
          Percent = 0;
          ProgressState = ProgressState.Normal;
          break;
        case LibraryOperation.UpdatePercentChanged:
          Percent = args.Percent ?? 0;
          break;
        case LibraryOperation.UpdateMangaChanged:
          switch (args.MangaOperation)
          {
            case MangaOperation.Added:
              break;
            case MangaOperation.Deleted:
              break;
            case MangaOperation.UpdateStarted:
              break;
            case MangaOperation.UpdateCompleted:
              TaskbarIcon.ShowInTray(Strings.Library_Status_MangaUpdate + args.Manga.Name + " завершено.", args.Manga);
              break;
            case MangaOperation.None:
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
          break;
        case LibraryOperation.UpdateCompleted:
          Percent = 0;
          ProgressState = ProgressState.None;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void SaveWindowState()
    {
      var window = ViewService.Instance.TryGet<System.Windows.Window>(this);
      if (window != null)
      {
        ConfigStorage.Instance.ViewConfig.SaveWindowState(window);
      }
    }

    private WindowModel()
    {
      Close = new ExitCommand();
      
      ProgressState = ProgressState.None;
    }

    private ProgressState beforePause = ProgressState.None;

    private void WindowOnClosing(Window sender, CancelEventArgs cancelEventArgs)
    {
      Close.Execute(sender);
    }

    private void WindowOnStateChanged(Window sender, EventArgs eventArgs)
    {
      if (ConfigStorage.Instance.AppConfig.MinimizeToTray && sender.WindowState == System.Windows.WindowState.Minimized)
        sender.Hide();
    }

    public void Refresh()
    {
      if (model != null && model.View != null)
        model.View.Refresh();
    }

    public void Dispose()
    {
      if (TaskbarIcon != null)
        TaskbarIcon.Dispose();
      model.Library.LibraryChanged -= LibraryOnLibraryChanged;
      model.Library.PropertyChanged -= LibraryOnPropertyChanged;
      //      if (lazyModel.IsValueCreated)
      //        lazyModel = new Lazy<WindowModel>(() => new WindowModel(new MainWindow()));
    }
  }
}