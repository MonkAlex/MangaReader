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
      HasMany(x => x.Container).AsBag().Cascade.AllDeleteOrphan().Not.LazyLoad();
      DiscriminateSubClassesOnColumn("Type");
    }
  }
}