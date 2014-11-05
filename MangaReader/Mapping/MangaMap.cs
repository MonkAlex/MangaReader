using FluentNHibernate.Mapping;
using MangaReader.Manga;
using MangaReader.Manga.Acomic;
using MangaReader.Manga.Grouple;

namespace MangaReader.Mapping
{

  public class MangaMap : ClassMap<Mangas>
  {
    public MangaMap()
    {
      Id(x => x.Id).GeneratedBy.Native();
      Map(x => x.Name).Not.LazyLoad();
      Map(x => x.Url).Not.LazyLoad();
      Map(x => x.Status).Not.LazyLoad();
      Map(x => x.NeedUpdate).Not.LazyLoad();
      Map(x => x.Doubles);
      DiscriminateSubClassesOnColumn(Mangas.Type);
    }
  }

  public class ReadmangaMap : SubclassMap<Readmanga>
  {
    public ReadmangaMap()
    {
      DiscriminatorValue(Readmanga.Type);
    }
  }

  public class AcomicsMap : SubclassMap<Acomics>
  {
    public AcomicsMap()
    {
      DiscriminatorValue(Acomics.Type);
    }
  }
}
