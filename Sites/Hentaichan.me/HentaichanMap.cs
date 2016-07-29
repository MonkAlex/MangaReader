using FluentNHibernate.Mapping;

namespace Hentaichan
{
  public class HentaichanMap : SubclassMap<Hentaichan>
  {
    public HentaichanMap()
    {
      Not.LazyLoad();
      DiscriminatorValue(Hentaichan.Type.ToString());
    }
  }

  public class HentaichanLoginMap : SubclassMap<HentaichanLogin>
  {
    public HentaichanLoginMap()
    {
      Not.LazyLoad();
      Map(x => x.UserId);
      Map(x => x.PasswordHash);
      DiscriminatorValue(HentaichanLogin.Type.ToString());
    }
  }
}