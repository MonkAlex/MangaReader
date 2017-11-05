using FluentNHibernate.Mapping;

namespace Hentaichan.NHibernate
{
  public class MangachanChapterMap : SubclassMap<Mangachan.MangachanChapter>
  {
    public MangachanChapterMap()
    {
      Not.LazyLoad();
      DiscriminatorValue("cdd3650e-f6a4-4f2b-9881-7e906ffc8650");
    }
  }
}