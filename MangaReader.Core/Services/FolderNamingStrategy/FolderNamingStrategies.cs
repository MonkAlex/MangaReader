using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{
  public static class FolderNamingStrategies
  {
    private static readonly Lazy<List<IFolderNamingStrategy>> StrategiesLazy = new Lazy<List<IFolderNamingStrategy>>(() =>
      new List<IFolderNamingStrategy>(Generic.GetAllTypes<IFolderNamingStrategy>().Select(Activator.CreateInstance).OfType<IFolderNamingStrategy>()));

    private static readonly ConcurrentDictionary<IManga, IFolderNamingStrategy> mangaCache = new ConcurrentDictionary<IManga, IFolderNamingStrategy>();

    public static IReadOnlyCollection<IFolderNamingStrategy> Strategies { get { return StrategiesLazy.Value.AsReadOnly(); } }

    public static IFolderNamingStrategy GetNamingStrategy(Guid id)
    {
      var selected = StrategiesLazy.Value.SingleOrDefault(s => s.Id == id);
      return selected ?? new LegacyFolderNaming();
    }

    public static IDisposable BlockStrategy(IManga manga)
    {
      return StrategyCache.Lock(manga);
    }

    public static IFolderNamingStrategy GetNamingStrategy(IManga manga)
    {
      if (mangaCache.TryGetValue(manga, out var strategy))
        return strategy;

      var settingId = manga.Setting.FolderNamingStrategy;
      if (Guid.Empty != settingId)
        return GetNamingStrategy(settingId);

      using (var context = Repository.GetEntityContext())
      {
        var config = context.Get<DatabaseConfig>().Single();
        return GetNamingStrategy(config.FolderNamingStrategy);
      }
    }

    private class StrategyCache : IDisposable
    {
      private IManga manga;

      public static StrategyCache Lock(IManga manga)
      {
        mangaCache.GetOrAdd(manga, GetNamingStrategy);
        return new StrategyCache(){manga = manga};
      }

      public void Dispose()
      {
        mangaCache.TryRemove(manga, out _);
      }
    }
  }
}