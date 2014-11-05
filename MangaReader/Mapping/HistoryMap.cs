using FluentNHibernate.Mapping;

namespace MangaReader.Mapping
{
  public class HistoryMap : ClassMap<MangaHistory>
  {
    public HistoryMap()
    {
      Id(x => x.Id).GeneratedBy.Native();
      Map(x => x.Url).Not.LazyLoad();
      Map(x => x.Date).Not.LazyLoad();
      References(x => x.Manga).Not.Nullable().Not.LazyLoad();
    }
  }
}
