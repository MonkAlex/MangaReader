using FluentNHibernate.Mapping;

namespace Grouple.NHibernate
{
  public class MintmangaMap : SubclassMap<Mintmanga>
  {
    public MintmangaMap()
    {
      Not.LazyLoad();
      DiscriminatorValue(MintmangaPlugin.Manga.ToString());
    }
  }
}