using FluentNHibernate.Mapping;

namespace Grouple.NHibernate
{
  public class GroupleMangaPageMap : SubclassMap<GroupleMangaPage>
  {
    public GroupleMangaPageMap()
    {
      Not.LazyLoad();
      DiscriminatorValue("2634fbff-c408-48cc-a7ea-2c9180032ce0");
    }
  }
}