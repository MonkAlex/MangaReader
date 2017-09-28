using FluentNHibernate.Mapping;
using MangaReader.Core.Manga;

namespace MangaReader.Core.NHibernate
{
  public class MangaPageMap : ClassMap<MangaPage>
  {
    public MangaPageMap()
    {
      Not.LazyLoad();
      Id(x => x.Id).GeneratedBy.Native();
      Map(x => x.Number);
      Map(x => x.Name);
      Map(x => x.Uri);
      Map(x => x.Folder);
      Map(x => x.ImageLink);
      DiscriminateSubClassesOnColumn("Type", "43dc8bf1-2756-48a7-a652-f0ee2d2f6192");
    }
  }
}