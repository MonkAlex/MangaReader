using FluentNHibernate.Mapping;

namespace Hentaichan.NHibernate
{
  public class HentaichanMap : SubclassMap<Hentaichan>
  {
    public HentaichanMap()
    {
      DiscriminatorValue(HentaichanPlugin.Manga.ToString());
    }
  }

  public class MangachanMap : SubclassMap<Mangachan.Mangachan>
  {
    public MangachanMap()
    {
      DiscriminatorValue(Mangachan.MangachanPlugin.Manga.ToString());
    }
  }
}