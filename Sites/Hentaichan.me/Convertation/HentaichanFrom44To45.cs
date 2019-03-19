using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using NHibernate.Linq;

namespace Hentaichan.Convertation
{
  public class HentaichanFrom44To45 : MangasConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && this.CanConvertVersion(process);
    }

    protected override async Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var mangas = await context.Get<Hentaichan>().ToListAsync().ConfigureAwait(false);
        foreach (var manga in mangas)
        {
          manga.Uri = new Uri(manga.Uri.OriginalString.Replace("/related/", "/manga/"));
          process.Percent += 100.0 / mangas.Count;
        }
        await mangas.SaveAll(context).ConfigureAwait(false);
      }
    }

    public HentaichanFrom44To45()
    {
      this.Version = new Version(1, 44, 11);
      this.CanReportProcess = true;
    }
  }
}