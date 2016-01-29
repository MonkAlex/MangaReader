using System;
using MangaReader.Services;
using MangaReader.Services.Config;

namespace Tests
{
  public class ReportProcess : IProcess
  {
    public double Percent { get; set; }
    public bool IsIndeterminate { get; set; }
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