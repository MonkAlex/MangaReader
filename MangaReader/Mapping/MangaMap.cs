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
      Map(x => x.LocalName).Not.LazyLoad();
      Map(x => x.ServerName).Not.LazyLoad();
      Map(x => x.IsNameChanged).Not.LazyLoad();
      Map(x => x.Uri).Not.LazyLoad();
      Map(x => x.Status).Not.LazyLoad();
      Map(x => x.NeedUpdate).Not.LazyLoad();
      Map(x => x.IsCompleted).Not.LazyLoad();
      Map(x => x.Folder).Not.LazyLoad();
      Map(x => x.NeedCompress).Not.LazyLoad();
      Map(x => x.CompressionMode).Not.LazyLoad();
      HasMany(x => x.Histories).Not.LazyLoad().AsBag().Cascade.AllDeleteOrphan();
      DiscriminateSubClassesOnColumn("Type");
    }
  }

  public class ReadmangaMap : SubclassMap<Readmanga>
  {
    public ReadmangaMap()
    {
      DiscriminatorValue(Readmanga.Type.ToString());
    }
  }

  public class AcomicsMap : SubclassMap<Acomics>
  {
    public AcomicsMap()
    {
      DiscriminatorValue(Acomics.Type.ToString());
    }
  }
}
