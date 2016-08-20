using FluentNHibernate.Mapping;

namespace Acomics.NHibernate
{
  public class AcomicsMap : SubclassMap<Acomics>
  {
    public AcomicsMap()
    {
      Not.LazyLoad();
      DiscriminatorValue(AcomicsPlugin.Manga.ToString());
    }
  }
}