using FluentNHibernate.Mapping;

namespace Hentaichan.NHibernate
{
  public class HentaichanLoginMap : SubclassMap<HentaichanLogin>
  {
    public HentaichanLoginMap()
    {
      Not.LazyLoad();
      Map(x => x.UserId);
      Map(x => x.PasswordHash);
      DiscriminatorValue("03ceff67-1472-438a-a90a-07b44f6ffdc4");
    }
  }
}