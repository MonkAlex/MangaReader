using FluentNHibernate.Mapping;

namespace Acomics.NHibernate
{
  public class AcomicsChapterMap : SubclassMap<AcomicsChapter>
  {
    public AcomicsChapterMap()
    {
      Not.LazyLoad();
      DiscriminatorValue("42a60c0f-d96e-45e3-9bdc-d2dfd2a309b5");
    }
  }
}