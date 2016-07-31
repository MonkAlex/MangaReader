using FluentNHibernate.Mapping;

namespace Acomics.NHibernate
{
  public class AcomicsLoginMap : SubclassMap<AcomicsLogin>
  {
    public AcomicsLoginMap()
    {
      Not.LazyLoad();
      Map(x => x.PasswordHash);
      DiscriminatorValue(AcomicsLogin.Type.ToString());
    }
  }
}