using FluentNHibernate.Mapping;

namespace Hentai2Read.com.NHibernate
{
  public class Hentai2ReadLoginMap : SubclassMap<Hentai2ReadLogin>
  {
    public Hentai2ReadLoginMap()
    {
      Not.LazyLoad();
      //Map(x => x.PasswordHash);
      DiscriminatorValue("426d5975-b4bb-4d24-abe5-44ec9b7e322c");
    }
  }
}