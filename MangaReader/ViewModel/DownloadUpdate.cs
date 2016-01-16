using System;
using System.Threading.Tasks;
using System.Windows;
using MangaReader.Services;
using MangaReader.Update;

namespace MangaReader.ViewModel
{
  public class DownloadUpdate : BaseViewModel, IProcess
  {
    private double percent;
    private bool isIndeterminate;
    private string status;
    private ConvertState state;
    private Window view;

    public DownloadUpdate(Window view) : base(view)
    {
      this.view = view;
    }

    public void Show()
    {
      view.ContentRendered += (sender, args) => Task.Run(() => Updater.StartUpdate(this));
      view.ShowDialog();
    }

    public double Percent
    {
      get { return percent; }
      set
      {
        percent = value;
        OnPropertyChanged();
      }
    }

    public bool IsIndeterminate
    {
      get { return isIndeterminate; }
      set
      {
        isIndeterminate = value;
        OnPropertyChanged();
      }
    }

    public string Status
    {
      get { return status; }
      set
      {
        status = value;
        OnPropertyChanged();
      }
    }

    public Version Version { get; set; }

    public ConvertState State
    {
      get { return state; }
      set
      {
        state = value;
        OnPropertyChanged();
        OnStateChanged(value);
      }
    }

    public event EventHandler<ConvertState> StateChanged;

    protected virtual void OnStateChanged(ConvertState e)
    {
      StateChanged?.Invoke(this, e);
    }
  }
}