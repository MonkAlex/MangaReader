using FluentNHibernate.Mapping;

namespace Grouple.NHibernate
{
  public class GroupleLoginMap : SubclassMap<GroupleLogin>
  {
    public GroupleLoginMap()
    {
      Not.LazyLoad();
      DiscriminatorValue("0bbe71b1-16e0-44f4-b7c6-3450e44e9a15");
    }
  }
}