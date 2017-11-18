using FluentNHibernate.Mapping;
using MangaReader.Core.Manga;

namespace MangaReader.Core.NHibernate
{
  public class VolumeMap : ClassMap<Volume>
  {
    public VolumeMap()
    {
      Not.LazyLoad();
      Id(x => x.Id).GeneratedBy.Native();
      Map(x => x.Number);
      Map(x => x.Name);
      Map(x => x.Uri);
      Map(x => x.Folder);
      Map(x => x.DownloadedAt);
      References(x => x.Manga).Class<Mangas>().Column($"{nameof(Mangas)}_id");
      HasMany(x => x.Container).AsBag().Cascade.AllDeleteOrphan().Not.LazyLoad();
      DiscriminateSubClassesOnColumn("Type", "4f1ac196-7ba8-4d0e-871d-5ab382fe1120");
    }
  }
}