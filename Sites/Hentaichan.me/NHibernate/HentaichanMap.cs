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
}