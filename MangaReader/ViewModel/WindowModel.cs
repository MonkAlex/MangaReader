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

        TaskbarIcon = new TaskbarIconModel(window.FindName("TaskbarIcon"));

        ConfigStorage.Instance.ViewConfig.UpdateWindowState(window);
        window.Show();
      }
      model = new MainPageModel();
      model.Show();
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
      UpdateAll = new UpdateAllCommand();
      Close = new ExitCommand();
      
      ProgressState = ProgressState.None;
      Library.UpdateStarted += LibraryOnUpdateStarted;
      Library.UpdateCompleted += LibraryOnUpdateCompleted;
      Library.UpdateMangaCompleted += LibraryOnUpdateMangaCompleted;
      Library.UpdatePercentChanged += LibraryOnUpdatePercentChanged;
      Library.PauseChanged += LibraryOnPauseChanged;
    }

    private ProgressState beforePause = ProgressState.None;

    private void LibraryOnPauseChanged(object sender, bool e)
    {
      if (e)
      {
        beforePause = ProgressState;
        ProgressState = ProgressState.Paused;
      }
      else
        ProgressState = beforePause;
    }

    private void LibraryOnUpdatePercentChanged(object sender, double i)
    {
      Percent = i;
    }

    private void LibraryOnUpdateMangaCompleted(object sender, IManga mangas)
    {
      TaskbarIcon.ShowInTray(Strings.Library_Status_MangaUpdate + mangas.Name + " завершено.", mangas);
    }

    private void LibraryOnUpdateCompleted(object sender, EventArgs eventArgs)
    {
      Percent = 0;
      ProgressState = ProgressState.None;
    }

    private void LibraryOnUpdateStarted(object sender, EventArgs eventArgs)
    {
      Percent = 0;
      ProgressState = ProgressState.Normal;
    }

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
      Library.UpdateStarted -= LibraryOnUpdateStarted;
      Library.UpdateCompleted -= LibraryOnUpdateCompleted;
      Library.UpdateMangaCompleted -= LibraryOnUpdateMangaCompleted;
      Library.UpdatePercentChanged -= LibraryOnUpdatePercentChanged;
      Library.PauseChanged -= LibraryOnPauseChanged;
      //      if (lazyModel.IsValueCreated)
      //        lazyModel = new Lazy<WindowModel>(() => new WindowModel(new MainWindow()));
    }
  }
}