using FluentNHibernate.Mapping;

namespace Acomics.NHibernate
{
  public class AcomicsLoginMap : SubclassMap<AcomicsLogin>
  {
    public AcomicsLoginMap()
    {
      Not.LazyLoad();
      Map(x => x.PasswordHash);
      DiscriminatorValue("f526cd85-7846-4f32-85a7-c57e3983dfb1");
    }
  }
}