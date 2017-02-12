using FluentNHibernate.Mapping;

namespace MangaReader.Core.NHibernate
{
  public class LoginMap : ClassMap<Account.ILogin>
  {
    public LoginMap()
    {
      Table(nameof(Account.Login));
      Not.LazyLoad();
      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.Password);
      Map(x => x.MainUri);
      DiscriminateSubClassesOnColumn("Type");
    }
  }
}
