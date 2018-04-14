using FluentNHibernate.Mapping;
using MangaReader.Core.Manga;

namespace MangaReader.Core.NHibernate
{

  public class MangaMap : ClassMap<Mangas>
  {
    public MangaMap()
    {
      Id(x => x.Id).GeneratedBy.Native();
      Map(x => x.Name);
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
      Map(x => x.Cover);
      Map(x => x.DownloadedAt);
      Map(x => x.Created);
      References(x => x.Setting).Column(nameof(Mangas.Setting));
      HasMany(x => x.Histories).CollectionType<HistoryBugType>().Cascade.AllDeleteOrphan();
      HasMany(x => x.Volumes).AsBag().Cascade.AllDeleteOrphan();
      HasMany(x => x.Chapters).AsBag().Cascade.AllDeleteOrphan();
      HasMany(x => x.Pages).AsBag().Cascade.AllDeleteOrphan();
      DiscriminateSubClassesOnColumn("Type");
    }
  }
}
