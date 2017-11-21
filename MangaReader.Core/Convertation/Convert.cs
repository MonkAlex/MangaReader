using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation
{

  public static class Converter
  {
    public static void Convert(IProcess process)
    {
      process.State = ConvertState.Started;

      Log.Add("Convert started.");

      Convert<ConfigConverter>(process);

      var mangaSettings = NHibernate.Repository.GetStateless<MangaSetting>();
      Log.AddFormat("Found {0} manga type settings:", mangaSettings.Count);
      mangaSettings.ForEach(s => Log.AddFormat("Load settings for {0}, guid {1}.", s.MangaName, s.Manga));

      Convert<MangasConverter>(process);
      Convert<DatabaseConverter>(process);
      Convert<HistoryConverter>(process);

      Log.Add("Convert completed.");

      using (var context = NHibernate.Repository.GetEntityContext())
      {
        var databaseConfig = context.Get<DatabaseConfig>().Single();
        var oldVersion = databaseConfig.Version;
        databaseConfig.Version = process.Version;
        databaseConfig.Save();
        process.State = ConvertState.Completed;

        if (process.Version.CompareTo(oldVersion) > 0 && context.Get<IManga>().Any())
          Client.OnClientUpdated(oldVersion);
      }
    }

    private static void Convert<T>(IProcess process) where T : BaseConverter
    {
      var converters = new List<T>(Generic.GetAllTypes<T>().Select(Activator.CreateInstance).OfType<T>());
      converters = converters.OrderBy(c => c.Version).ToList();
      foreach (var converter in converters)
      {
        process.Status = converter.Name;
        process.Percent = 0;
        process.ProgressState = converter.CanReportProcess ? ProgressState.Normal : ProgressState.Indeterminate;
        converter.Convert(process);
      }
    }
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