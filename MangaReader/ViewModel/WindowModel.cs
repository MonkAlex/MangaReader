using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MangaReader.Core.Services.Config;
using MangaReader.Services.Config;
using MangaReader.UI;
using MangaReader.UI.MainForm;
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
      var window = ViewService.Instance.TryGet(this);
      if (window != null)
      {
        window.StateChanged += (o, a) => WindowOnStateChanged(window, a);
        window.Closing += (o, a) => WindowOnClosing(window, a);

        TaskbarIcon = new TaskbarIconModel(window.FindName("TaskbarIcon"));

        ConfigStorage.Instance.ViewConfig.UpdateWindowState(window);
        window.Show();
      }
    }

    public void SaveWindowState()
    {
      var window = ViewService.Instance.TryGet(this);
      if (window != null)
      {
        ConfigStorage.Instance.ViewConfig.SaveWindowState(window);
      }
    }

    private WindowModel()
    {
      UpdateAll = new UpdateAllCommand();
      Close = new ExitCommand();
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

    public void Dispose()
    {
      if (TaskbarIcon != null)
        TaskbarIcon.Dispose();
//      if (lazyModel.IsValueCreated)
//        lazyModel = new Lazy<WindowModel>(() => new WindowModel(new MainWindow()));
    }
  }
}