using FluentNHibernate.Mapping;

namespace Grouple.NHibernate
{
  public class ReadmangaMap : SubclassMap<Readmanga>
  {
    public ReadmangaMap()
    {
      DiscriminatorValue(ReadmangaPlugin.Manga.ToString());
    }
  }
}