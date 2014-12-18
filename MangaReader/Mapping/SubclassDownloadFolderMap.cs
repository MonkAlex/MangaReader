using FluentNHibernate.Mapping;
using MangaReader.Services;

namespace MangaReader.Mapping
{
  public class SubclassDownloadFolderMap : ClassMap<Settings.SubclassDownloadFolder>
  {
    public SubclassDownloadFolderMap()
    {
      Id(x => x.Id);
      Map(x => x.TypeName).Not.LazyLoad().Unique();
      Map(x => x.Folder).Not.LazyLoad();
    }
  }
}
