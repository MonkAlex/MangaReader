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
      DiscriminatorValue("03CEFF67-1472-438A-A90A-07B44F6FFDC4");
    }
  }
}