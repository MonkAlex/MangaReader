using System;
using Avalonia.Threading;
using MangaReader.Avalonia.ViewModel;
using MangaReader.Core.Convertation;
using MangaReader.Core.Services.Config;

namespace MangaReader.Avalonia
{
  public class ProcessModel : ViewModelBase, IProcess
  {
    private double percent;
    private ConvertState state;
    private string status;
    private ProgressState progressState;

    public event EventHandler<ConvertState> StateChanged;

    public double Percent
    {
      get { return percent; }
      set
      {
        percent = value;
        this.RaisePropertyChanged();
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
        RaisePropertyChanged();
        RaisePropertyChanged(nameof(IsIndeterminate));
      }
    }

    public string Status
    {
      get { return status; }
      set
      {
        status = value;
        RaisePropertyChanged();
      }
    }

    public Version Version { get; set; }

    public ConvertState State
    {
      get { return state; }
      set
      {
        state = value;
        RaisePropertyChanged();
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
      if (Dispatcher.UIThread.CheckAccess())
        StateChanged?.Invoke(this, newState);
      else
        Dispatcher.UIThread.InvokeAsync(() => StateChanged?.Invoke(this, newState));
    }

  }
}