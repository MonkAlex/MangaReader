using FluentNHibernate.Mapping;

namespace Grouple.NHibernate
{
  public class MintmangaMap : SubclassMap<Mintmanga>
  {
    public MintmangaMap()
    {
      DiscriminatorValue(MintmangaPlugin.Manga.ToString());
    }
  }
}