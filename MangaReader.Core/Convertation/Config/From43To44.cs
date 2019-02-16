using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation.Config
{
  public class From43To44 : ConfigConverter
  {
    protected override Task ProtectedConvert(IProcess process)
    {
      var settings = NHibernate.Repository.GetStateless<MangaSetting>();
      foreach (var setting in settings)
      {
        this.RunSql($"update Mangas set Setting = {setting.Id} where Setting is null and Type = \"{setting.Manga}\"");
      }

      using (var context = NHibernate.Repository.GetEntityContext())
      {
        var config = context.Get<DatabaseConfig>().SingleOrCreate();
        if (context.Get<IManga>().Any())
          config.FolderNamingStrategy = Generic.GetNamingStrategyId<LegacyFolderNaming>();
        context.Save(config);
      }

      return Task.CompletedTask;
    }

    public From43To44()
    {
      this.Version = new Version(1, 43, 4);
    }
  }
}