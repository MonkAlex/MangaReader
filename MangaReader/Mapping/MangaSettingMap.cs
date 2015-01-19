using FluentNHibernate.Mapping;
using MangaReader.Services;

namespace MangaReader.Mapping
{
  public class MangaSettingMap : ClassMap<MangaSetting>
  {
    public MangaSettingMap()
    {
      Id(x => x.Id);
      Map(x => x.Manga).Not.LazyLoad().Unique();
      Map(x => x.Folder).Not.LazyLoad();
      Map(x => x.MangaName).Not.LazyLoad();
      References(x => x.Login).Not.LazyLoad().Cascade.SaveUpdate();
    }
  }
}
