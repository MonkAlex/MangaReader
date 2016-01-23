using System;
using System.Windows;
using MangaReader.Services;
using MangaReader.Services.Config;

namespace MangaReader.ViewModel
{
  public class ProcessModel : BaseViewModel, IProcess
  {
    private double percent;
    private bool isIndeterminate;
    private ConvertState state;
    private string status;
    protected readonly Window view;

    public event EventHandler<ConvertState> StateChanged;

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

    public ProcessModel(Window view) : base(view)
    {
      this.view = view;
      this.Percent = 0;
      this.IsIndeterminate = true;
      this.Status = string.Empty;
      this.Version = new Version(AppConfig.Version.Major, AppConfig.Version.Minor, AppConfig.Version.Build);
      this.State = ConvertState.None;
    }

    protected virtual void OnStateChanged(ConvertState newState)
    {
      if (Application.Current.Dispatcher.CheckAccess())
        StateChanged?.Invoke(this, newState);
      else
        Application.Current.Dispatcher.InvokeAsync(() => StateChanged?.Invoke(this, newState));
    }

  }
}