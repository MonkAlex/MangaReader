using FluentNHibernate.Mapping;

namespace Grouple.NHibernate
{
  public class GroupleChapterMap : SubclassMap<GroupleChapter>
  {
    public GroupleChapterMap()
    {
      Not.LazyLoad();
      DiscriminatorValue("c4f3c060-095d-4c50-8bc5-7cb9b0d7183c");
    }
  }
}