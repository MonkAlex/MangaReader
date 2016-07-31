using FluentNHibernate.Mapping;

namespace MangaReader.Core.NHibernate
{
  public class LoginMap : ClassMap<Account.Login>
  {
    public LoginMap()
    {
      Not.LazyLoad();
      Id(x => x.Id);
      Map(x => x.Name);
      Map(x => x.Password);
      Map(x => x.MainUri);
      DiscriminateSubClassesOnColumn("Type");
    }
  }

  public class GroupleLoginMap : SubclassMap<Manga.Grouple.GroupleLogin>
  {
    public GroupleLoginMap()
    {
      Not.LazyLoad();
      DiscriminatorValue(Manga.Grouple.GroupleLogin.Type.ToString());
    }
  }
}
