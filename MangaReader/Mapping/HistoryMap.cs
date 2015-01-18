using FluentNHibernate.Mapping;

namespace MangaReader.Mapping
{
  public class HistoryMap : ClassMap<MangaHistory>
  {
    public HistoryMap()
    {
      Id(x => x.Id).GeneratedBy.Native();
      Map(x => x.Uri).Not.LazyLoad();
      Map(x => x.Date).Not.LazyLoad();
    }
  }
}
