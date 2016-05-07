using FluentNHibernate.Mapping;
using MangaReader.Core.Manga;

namespace MangaReader.Core.NHibernate
{
  public class HistoryMap : ClassMap<MangaHistory>
  {
    public HistoryMap()
    {
      Not.LazyLoad();
      Id(x => x.Id).GeneratedBy.Native();
      Map(x => x.Uri);
      Map(x => x.Date);
    }
  }
}
