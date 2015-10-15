using FluentNHibernate.Mapping;

namespace MangaReader.Mapping
{
  public class LoginMap : ClassMap<Account.Login>
  {
    public LoginMap()
    {
      Id(x => x.Id);
      Map(x => x.Name).Not.LazyLoad();
      Map(x => x.Password).Not.LazyLoad();
      DiscriminateSubClassesOnColumn("Type");
    }
  }

  public class HentaichanLoginMap : SubclassMap<Manga.Hentaichan.HentaichanLogin>
  {
    public HentaichanLoginMap()
    {
      Map(x => x.UserId).Not.LazyLoad();
      Map(x => x.PasswordHash).Not.LazyLoad();
      DiscriminatorValue(Manga.Hentaichan.HentaichanLogin.Type.ToString());
    }
  }

  public class GroupleLoginMap : SubclassMap<Manga.Grouple.GroupleLogin>
  {
    public GroupleLoginMap()
    {
      DiscriminatorValue(Manga.Grouple.GroupleLogin.Type.ToString());
    }
  }

  public class AcomicsLoginMap : SubclassMap<Manga.Acomic.AcomicsLogin>
  {
    public AcomicsLoginMap()
    {
      DiscriminatorValue(Manga.Acomic.AcomicsLogin.Type.ToString());
    }
  }
}
