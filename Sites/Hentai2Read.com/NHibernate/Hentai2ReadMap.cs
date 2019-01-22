using FluentNHibernate.Mapping;

namespace Hentai2Read.com.NHibernate
{
  public class Hentai2ReadMap : SubclassMap<Hentai2Read>
  {
    public Hentai2ReadMap()
    {
      DiscriminatorValue(Hentai2ReadPlugin.Manga.ToString());
    }
  }
}