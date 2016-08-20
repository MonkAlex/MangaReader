using FluentNHibernate.Mapping;

namespace Acomics.NHibernate
{
  public class AcomicsLoginMap : SubclassMap<AcomicsLogin>
  {
    public AcomicsLoginMap()
    {
      Not.LazyLoad();
      Map(x => x.PasswordHash);
      DiscriminatorValue("F526CD85-7846-4F32-85A7-C57E3983DFB1");
    }
  }
}