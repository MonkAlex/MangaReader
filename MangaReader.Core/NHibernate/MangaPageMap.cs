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
      Map(x => x.ImageLink);
      Map(x => x.DownloadedAt);
      References(x => x.Manga).Class<Mangas>().Column($"{nameof(Mangas)}_id");
      References(x => x.Chapter).Column($"{nameof(Chapter)}_id");
      DiscriminateSubClassesOnColumn("Type", "43dc8bf1-2756-48a7-a652-f0ee2d2f6192");
    }
  }
}