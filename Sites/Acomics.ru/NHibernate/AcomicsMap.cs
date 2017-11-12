using FluentNHibernate.Mapping;

namespace Acomics.NHibernate
{
  public class AcomicsMap : SubclassMap<Acomics>
  {
    public AcomicsMap()
    {
      DiscriminatorValue(AcomicsPlugin.Manga.ToString());
    }
  }
}