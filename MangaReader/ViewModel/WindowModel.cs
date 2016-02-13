using System;
using System.Windows;
using System.Windows.Input;
using MangaReader.UI;
using MangaReader.ViewModel.Commands;

namespace MangaReader.ViewModel
{
  public class WindowModel : BaseViewModel, IDisposable
  {
    private static Lazy<WindowModel> lazyModel = new Lazy<WindowModel>(() => new WindowModel(new MainWindow())); 
    public static WindowModel Instance { get { return lazyModel.Value; } }

    protected internal Window window { get { return this.view as Window; } }
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
      window.Show();
    }

    private WindowModel(Window window) : base(window)
    {
      UpdateAll = new UpdateAllCommand();
      Close = new ExitCommand();

      TaskbarIcon = new TaskbarIconModel(this.view.FindName("TaskbarIcon"));
    }

    public void Dispose()
    {
      if (TaskbarIcon != null)
        TaskbarIcon.Dispose();
      if (lazyModel.IsValueCreated)
        lazyModel = new Lazy<WindowModel>(() => new WindowModel(new MainWindow()));
    }
  }
}