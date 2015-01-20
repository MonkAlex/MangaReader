using FluentNHibernate.Mapping;
using MangaReader.Account;

namespace MangaReader.Mapping
{
  public class LoginMap : ClassMap<Login>
  {
    public LoginMap()
    {
      Id(x => x.Id);
      Map(x => x.Name).Not.LazyLoad();
      Map(x => x.Password).Not.LazyLoad();
    }
  }
}
