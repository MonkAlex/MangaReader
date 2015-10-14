using System;
using System.Reflection;
using System.Threading;

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
    internal double Percent = 0;
    internal bool IsIndeterminate = true;
    internal string Status = string.Empty;
    internal Version Version = Settings.DatabaseVersion;
  }

  public static class Converter
  {
    public static ConverterProcess Process = new ConverterProcess() 
    { Version = new Version(Settings.AppVersion.Major, Settings.AppVersion.Minor, Settings.AppVersion.Build ) };

    public static ConverterState State = ConverterState.None;

    static public void Convert(bool withDialog)
    {
      var dialog = new Converting();

      if (withDialog)
        dialog.ShowDialog(JustConvert);
      else
        JustConvert();

      if (withDialog)
      {
        while (State != ConverterState.Completed) { Thread.Sleep(100); }
      }
    }

    static private void JustConvert()
    {
      State = ConverterState.Started;

      Log.Add("Convert started.");

      Process.Status = "Convert settings...";
      Settings.Convert();
      Settings.Load();

      Process.Status = "Convert manga...";
#pragma warning disable 618
      Cache.Convert(Process);
#pragma warning restore 618
      Mapping.Converting.ConvertAll(Process);

      Process.Status = "Convert history...";
      Process.Percent = 0;
      History.Convert(Process);

      Process.Status = "Convert manga list...";
      Process.Percent = 0;
      Library.Convert(Process);
      Log.Add("Convert completed.");

      Settings.DatabaseVersion = Process.Version;
      State = ConverterState.Completed;
    }
  }
}
