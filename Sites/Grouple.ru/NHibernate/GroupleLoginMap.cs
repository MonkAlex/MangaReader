using FluentNHibernate.Mapping;

namespace Grouple.NHibernate
{
  public class GroupleLoginMap : SubclassMap<GroupleLogin>
  {
    public GroupleLoginMap()
    {
      Not.LazyLoad();
      DiscriminatorValue("0BBE71B1-16E0-44F4-B7C6-3450E44E9A15");
    }
  }
}