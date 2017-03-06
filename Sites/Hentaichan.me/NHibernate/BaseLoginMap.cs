using FluentNHibernate.Mapping;

namespace Hentaichan.NHibernate
{
  public class BaseLoginMap : SubclassMap<BaseLogin>
  {
    public BaseLoginMap()
    {
      Not.LazyLoad();
      Map(x => x.UserId);
      Map(x => x.PasswordHash);
    }
  }

  public class HentaichanLoginMap : SubclassMap<HentaichanLogin>
  {
    public HentaichanLoginMap()
    {
      Not.LazyLoad();
      DiscriminatorValue("03ceff67-1472-438a-a90a-07b44f6ffdc4");
    }
  }

  public class MangachanLoginMap : SubclassMap<Mangachan.MangachanLogin>
  {
    public MangachanLoginMap()
    {
      Not.LazyLoad();
      DiscriminatorValue("07205c73-6e4b-4b6c-8e52-bb9cd245663d");
    }
  }
}