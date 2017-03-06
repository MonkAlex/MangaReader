using FluentNHibernate.Mapping;

namespace Hentaichan.NHibernate
{
  public class HentaichanMap : SubclassMap<Hentaichan>
  {
    public HentaichanMap()
    {
      Not.LazyLoad();
      DiscriminatorValue(HentaichanPlugin.Manga.ToString());
    }
  }

  public class MangachanMap : SubclassMap<Mangachan.Mangachan>
  {
    public MangachanMap()
    {
      Not.LazyLoad();
      DiscriminatorValue(Mangachan.MangachanPlugin.Manga.ToString());
    }
  }
}