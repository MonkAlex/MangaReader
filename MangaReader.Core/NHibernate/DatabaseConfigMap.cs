using FluentNHibernate.Mapping;
using MangaReader.Services.Config;

namespace MangaReader.Core.NHibernate
{
  public class DatabaseConfigMap : ClassMap<DatabaseConfig>
  {
    public DatabaseConfigMap()
    {
      Not.LazyLoad();
      Id(x => x.Id);
      Map(x => x.Version);
    }
  }
}