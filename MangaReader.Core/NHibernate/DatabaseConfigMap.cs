using FluentNHibernate.Mapping;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.NHibernate
{
  public class DatabaseConfigMap : ClassMap<DatabaseConfig>
  {
    public DatabaseConfigMap()
    {
      Not.LazyLoad();
      Id(x => x.Id);
      Map(x => x.Version);
      Map(x => x.UniqueId);
    }
  }
}