using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MangaReader.Core.Services.Config;
using MangaReader.Services.Config;
using MangaReader.UI;
using MangaReader.UI.MainForm;
using MangaReader.ViewModel.Commands;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class WindowModel : ProcessModel, IDisposable
  {
    private static Lazy<WindowModel> lazyModel = new Lazy<WindowModel>(() => new WindowModel(new MainWindow())); 
    public static WindowModel Instance { get { return lazyModel.Value; } }

    private object content;
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
      this.Content = new Table();
      ConfigStorage.Instance.ViewConfig.UpdateWindowState(window);
      window.Show();
    }

    public void SaveWindowState()
    {
      ConfigStorage.Instance.ViewConfig.SaveWindowState(window);
    }

    private WindowModel(Window window) : base(window)
    {
      UpdateAll = new UpdateAllCommand();
      Close = new ExitCommand();

      window.StateChanged += WindowOnStateChanged;
      window.Closing += WindowOnClosing;

      TaskbarIcon = new TaskbarIconModel(this.view.FindName("TaskbarIcon"));
    }

    private void WindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
    {
      Close.Execute(window);
    }

    private void WindowOnStateChanged(object sender, EventArgs eventArgs)
    {
      if (ConfigStorage.Instance.AppConfig.MinimizeToTray && window.WindowState == System.Windows.WindowState.Minimized)
        window.Hide();
    }

    public void Dispose()
    {
      if (TaskbarIcon != null)
        TaskbarIcon.Dispose();
//      if (lazyModel.IsValueCreated)
//        lazyModel = new Lazy<WindowModel>(() => new WindowModel(new MainWindow()));
    }
  }
}