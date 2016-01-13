using System;
using System.Threading;
using MangaReader.Services.Config;

namespace MangaReader.Services
{

  public enum ConverterState
  {
    None,
    Started,
    Completed
  }

  public class ConverterProcess
  {
    public double Percent { get; set; }
    public bool IsIndeterminate { get; set; }
    public string Status { get; set; }
    public Version Version { get; set; }

    public ConverterProcess(Version version)
    {
      this.Percent = 0;
      this.IsIndeterminate = true;
      this.Status = string.Empty;
      this.Version = version;
    }

    public ConverterProcess()
    {
      this.Percent = 0;
      this.IsIndeterminate = true;
      this.Status = string.Empty;
      this.Version = ConfigStorage.Instance.DatabaseConfig.Version;
    }
  }

  public static class Converter
  {
    public static ConverterProcess Process = new ConverterProcess(new Version(AppConfig.Version.Major, AppConfig.Version.Minor, AppConfig.Version.Build));

    public static ConverterState State = ConverterState.None;

    static public void Convert()
    {
      OnConvertStarted();

      JustConvert();

      OnConvertCompleted();
    }

    static private void JustConvert()
    {
      State = ConverterState.Started;

      Log.Add("Convert started.");

      Process.Status = "Convert settings...";
      Config.Converter.ConvertAll(Process);

      Process.Status = "Convert manga...";
#pragma warning disable 618
      Cache.Convert(Process);
#pragma warning restore 618
      Mapping.Converting.ConvertAll(Process);

      Process.Status = "Convert manga list...";
      Process.Percent = 0;
      Library.Convert(Process);

      Process.Status = "Convert history...";
      Process.Percent = 0;
      History.ConvertAll(Process);

      Log.Add("Convert completed.");

      ConfigStorage.Instance.DatabaseConfig.Version = Process.Version;
      State = ConverterState.Completed;
    }


    #region Events

    public static event EventHandler<Action> ConvertStarted;

    public static event EventHandler ConvertCompleted;

    private static void OnConvertStarted()
    {
      ConvertStarted?.Invoke(null, JustConvert);
    }

    private static void OnConvertCompleted()
    {
      ConvertCompleted?.Invoke(null, EventArgs.Empty);
    }
    
    #endregion
  }
}
