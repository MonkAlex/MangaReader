using FluentNHibernate.Mapping;
using MangaReader.Core.Manga;

namespace MangaReader.Core.NHibernate
{
  public class ChapterMap : ClassMap<Chapter>
  {
    public ChapterMap()
    {
      Not.LazyLoad();
      Id(x => x.Id).GeneratedBy.Native();
      Map(x => x.Number);
      Map(x => x.Name);
      Map(x => x.Uri);
      Map(x => x.Folder);
      HasMany(x => x.Container).CollectionType<uNhAddIns.WPF.Collections.Types.ObservableBagType<MangaPage>>().Cascade.AllDeleteOrphan().Not.LazyLoad();
      DiscriminateSubClassesOnColumn("Type", "1c63706d-a375-4154-ad17-e55be1eed4fe");
    }
  }
}