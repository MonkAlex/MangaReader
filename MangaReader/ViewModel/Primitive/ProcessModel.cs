using System;
using System.Windows;
using System.Windows.Shell;
using MangaReader.Core.Convertation;
using MangaReader.Core.Services.Config;

namespace MangaReader.ViewModel.Primitive
{
  public class ProcessModel : BaseViewModel, IProcess
  {
    private double percent;
    private ConvertState state;
    private string status;
    private ProgressState progressState;
    private TaskbarItemProgressState taskbarItemProgressState;

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
      get { return this.ProgressState == ProgressState.Indeterminate; }
    }

    public ProgressState ProgressState
    {
      get { return progressState; }
      set
      {
        progressState = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(IsIndeterminate));
        Enum.TryParse(value.ToString(), out taskbarItemProgressState);
        OnPropertyChanged(nameof(TaskbarItemProgressState));
      }
    }

    public TaskbarItemProgressState TaskbarItemProgressState { get { return taskbarItemProgressState; } }

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

    public ProcessModel()
    {
      this.Percent = 0;
      this.ProgressState = ProgressState.Indeterminate;
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