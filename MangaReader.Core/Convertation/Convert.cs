using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation
{

  public static class Converter
  {
    public static async Task Convert(IProcess process)
    {
      process.State = ConvertState.Started;

      Log.Add("Convert started.");

      await Convert<ConfigConverter>(process).ConfigureAwait(false);

      var mangaSettings = NHibernate.Repository.GetStateless<MangaSetting>();
      Log.AddFormat("Found {0} manga type settings:", mangaSettings.Count);
      mangaSettings.ForEach(s => Log.AddFormat("Load settings for {0}, guid {1}.", s.MangaName, s.Manga));

      var converters = new List<BaseConverter>(Generic
        .GetAllTypes<BaseConverter>().Where(t => !typeof(ConfigConverter).IsAssignableFrom(t)).Select(Activator.CreateInstance).OfType<BaseConverter>());
      await ConvertImpl(process, converters).ConfigureAwait(false);

      Log.Add("Convert completed.");

      using (var context = NHibernate.Repository.GetEntityContext("Actualize app version in db"))
      {
        var databaseConfig = context.Get<DatabaseConfig>().Single();
        var oldVersion = databaseConfig.Version;
        databaseConfig.Version = process.Version;
        context.Save(databaseConfig);
        process.State = ConvertState.Completed;

        if (process.Version.CompareTo(oldVersion) > 0 && context.Get<IManga>().Any())
          Client.OnClientUpdated(oldVersion);
      }
    }

    private static async Task Convert<T>(IProcess process) where T : BaseConverter
    {
      var converters = new List<T>(Generic.GetAllTypes<T>().Select(Activator.CreateInstance).OfType<T>());
      await ConvertImpl(process, converters).ConfigureAwait(false);
    }

    private static async Task ConvertImpl<T>(IProcess process, List<T> converters) where T : BaseConverter
    {
      converters = converters.OrderBy(c => c.Version).ToList();
      foreach (var converter in converters)
      {
        process.Status = converter.Name;
        process.Percent = 0;
        process.ProgressState = converter.CanReportProcess ? ProgressState.Normal : ProgressState.Indeterminate;
        await converter.Convert(process).ConfigureAwait(false);
      }

      process.ProgressState = ProgressState.Indeterminate;
      process.Status = string.Empty;
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