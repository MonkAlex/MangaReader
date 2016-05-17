using System;
using MangaReader.Core.Convertation;
using MangaReader.Core.Services.Config;

namespace Tests
{
  public class ReportProcess : IProcess
  {
    public double Percent { get; set; }
    public ProgressState ProgressState { get; set; }
    public string Status { get; set; }
    public Version Version { get; set; }
    public ConvertState State { get; set; }
    public event EventHandler<ConvertState> StateChanged;

    public ReportProcess()
    {
      Version = AppConfig.Version;
      State = ConvertState.None;
    }

    protected virtual void OnStateChanged(ConvertState e)
    {
      StateChanged?.Invoke(this, e);
    }
  }
}