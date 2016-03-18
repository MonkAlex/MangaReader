using FluentNHibernate.Mapping;
using MangaReader.Manga;
using MangaReader.Manga.Acomic;
using MangaReader.Manga.Grouple;
using MangaReader.Manga.Hentaichan;

namespace MangaReader.Core.NHibernate
{

  public class MangaMap : ClassMap<Mangas>
  {
    public MangaMap()
    {
      Not.LazyLoad();
      Id(x => x.Id).GeneratedBy.Native();
      Map(x => x.LocalName);
      Map(x => x.ServerName);
      Map(x => x.IsNameChanged);
      Map(x => x.HasVolumes);
      Map(x => x.HasChapters);
      Map(x => x.Uri);
      Map(x => x.Status);
      Map(x => x.NeedUpdate);
      Map(x => x.IsCompleted);
      Map(x => x.Folder);
      Map(x => x.NeedCompress);
      Map(x => x.CompressionMode);
      HasMany(x => x.Histories).AsBag().Cascade.AllDeleteOrphan();
      DiscriminateSubClassesOnColumn("Type");
    }
  }

  public class ReadmangaMap : SubclassMap<Readmanga>
  {
    public ReadmangaMap()
    {
      Not.LazyLoad();
      DiscriminatorValue(Readmanga.Type.ToString());
    }
  }

  public class AcomicsMap : SubclassMap<Acomics>
  {
    public AcomicsMap()
    {
      Not.LazyLoad();
      DiscriminatorValue(Acomics.Type.ToString());
    }
  }

  public class HentaichanMap : SubclassMap<Hentaichan>
  {
    public HentaichanMap()
    {
      Not.LazyLoad();
      DiscriminatorValue(Hentaichan.Type.ToString());
    }
  }
}
