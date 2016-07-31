using FluentNHibernate.Mapping;

namespace Grouple.NHibernate
{
  public class GroupleLoginMap : SubclassMap<GroupleLogin>
  {
    public GroupleLoginMap()
    {
      Not.LazyLoad();
      DiscriminatorValue(GroupleLogin.Type.ToString());
    }
  }
}