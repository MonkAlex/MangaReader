using System;
using MangaReader.Services.Config;

namespace MangaReader.Services
{

  public static class Converter
  {
    static public void Convert(IProcess process)
    {
      process.State = ConvertState.Started;

      Log.Add("Convert started.");

      process.Status = "Проверка настроек...";
      Config.Converter.ConvertAll(process);

      process.Status = "Конвертация манги...";
#pragma warning disable 618
      Cache.Convert(process);
#pragma warning restore 618
      Mapping.Converting.ConvertAll(process);

      process.Status = "Конвертация манги...";
      process.Percent = 0;
      Library.Convert(process);

      process.Status = "Конвертация истории...";
      process.Percent = 0;
      History.ConvertAll(process);

      Log.Add("Convert completed.");

      ConfigStorage.Instance.DatabaseConfig.Version = process.Version;
      process.State = ConvertState.Completed;
    }
  }

  public interface IProcess
  {
    double Percent { get; set; }

    bool IsIndeterminate { get; set; }

    string Status { get; set; }

    Version Version { get; set; }

    ConvertState State { get; set; }

    event EventHandler<ConvertState> StateChanged;
  }

  public enum ConvertState
  {
    None,
    Started,
    Completed
  }
}