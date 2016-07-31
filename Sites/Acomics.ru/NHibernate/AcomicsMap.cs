using FluentNHibernate.Mapping;

namespace Acomics.NHibernate
{
  public class AcomicsMap : SubclassMap<Acomics>
  {
    public AcomicsMap()
    {
      Not.LazyLoad();
      DiscriminatorValue(Acomics.Type.ToString());
    }
  }
}