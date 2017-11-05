using FluentNHibernate.Mapping;

namespace Hentaichan.NHibernate
{
  public class HentaichanChapterMap : SubclassMap<HentaichanChapter>
  {
    public HentaichanChapterMap()
    {
      Not.LazyLoad();
      DiscriminatorValue("f9aa864a-3f73-4501-8e49-575c76dd7de3");
    }
  }
}