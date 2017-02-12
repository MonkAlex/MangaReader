using FluentNHibernate.Mapping;

namespace Grouple.NHibernate
{
  public class ReadmangaMap : SubclassMap<Readmanga>
  {
    public ReadmangaMap()
    {
      Not.LazyLoad();
      DiscriminatorValue(ReadmangaPlugin.Manga.ToString());
    }
  }
}