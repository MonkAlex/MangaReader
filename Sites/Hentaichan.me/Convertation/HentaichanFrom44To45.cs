using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;

namespace Hentaichan.Convertation
{
  public class HentaichanFrom44To45 : MangasConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && this.CanConvertVersion(process);
    }

    protected override Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var mangas = context.Get<Hentaichan>().ToList();
        foreach (var manga in mangas)
        {
          manga.Uri = new Uri(manga.Uri.OriginalString.Replace("/related/", "/manga/"));
          process.Percent += 100.0 / mangas.Count;
        }
        mangas.SaveAll(context);
      }

      return Task.CompletedTask;
    }

    public HentaichanFrom44To45()
    {
      this.Version = new Version(1, 44, 11);
      this.CanReportProcess = true;
    }
  }
}