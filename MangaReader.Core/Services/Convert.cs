using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{

  public static class Converter
  {
    static public void Convert(IProcess process)
    {
      process.State = ConvertState.Started;

      Log.Add("Convert started.");

      process.Status = "Проверка настроек...";
      Convert<ConfigConverter>(process);

      process.Status = "Конвертация манги...";
      Convert<CacheConverter>(process);

      Converting.ConvertAll(process);

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

    private static void Convert<T>(IProcess process) where T : BaseConverter
    {
      var converters = new List<T>();
      foreach (var assembly in ResolveAssembly.AllowedAssemblies())
      {
        converters.AddRange(assembly.GetTypes()
          .Where(t => !t.IsAbstract && t.IsAssignableFrom(typeof(T)))
          .Select(Activator.CreateInstance)
          .OfType<T>());
      }
      converters = converters.OrderBy(c => c.Version).ToList();
      foreach (var converter in converters)
      {
        converter.Convert(process);
      }
    }
  }

  public interface IProcess
  {
    double Percent { get; set; }

    ProgressState ProgressState { get; set; } 

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

  /// <summary>
  /// Определяет состояние индикатора хода выполнения на панели задач Windows.
  /// </summary>
  public enum ProgressState
  {
    None,
    Indeterminate,
    Normal,
    Error,
    Paused,
  }
}