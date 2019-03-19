using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using NHibernate.Linq;

namespace MangaReader.Core.Convertation.Config
{
  public class From43To44 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      var settings = NHibernate.Repository.GetStateless<MangaSetting>();
      foreach (var setting in settings)
      {
        await RunSql($"update Mangas set Setting = {setting.Id} where Setting is null and Type = \"{setting.Manga}\"")
          .ConfigureAwait(false);
      }

      using (var context = NHibernate.Repository.GetEntityContext())
      {
        var config = await context.Get<DatabaseConfig>().SingleOrCreate().ConfigureAwait(false);
        if (await context.Get<IManga>().AnyAsync().ConfigureAwait(false))
          config.FolderNamingStrategy = Generic.GetNamingStrategyId<LegacyFolderNaming>();
        await context.Save(config).ConfigureAwait(false);
      }
    }

    public From43To44()
    {
      this.Version = new Version(1, 43, 4);
    }
  }
}