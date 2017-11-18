using FluentNHibernate.Mapping;
using MangaReader.Core.Services;

namespace MangaReader.Core.NHibernate
{
  public class MangaSettingMap : ClassMap<MangaSetting>
  {
    public MangaSettingMap()
    {
      Not.LazyLoad();
      Id(x => x.Id);
      Map(x => x.Manga).Unique();
      Map(x => x.Folder);
      Map(x => x.MangaName);
      Map(x => x.CompressManga);
      Map(x => x.OnlyUpdate);
      Map(x => x.DefaultCompression);
      Map(x => x.MainUri);
      Map(x => x.FolderNamingStrategy);
      HasMany(x => x.MangaSettingUris).Element("Uri").Not.LazyLoad();
      References(x => x.Login).Cascade.All().NotFound.Ignore();
    }
  }
}
