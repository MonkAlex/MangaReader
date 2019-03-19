using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using NHibernate.Linq;

namespace Hentaichan
{
  public class Hentaichan : Mangas
  {
    public override List<Compression.CompressionMode> AllowedCompressionModes
    {
      get { return base.AllowedCompressionModes.Where(m => !Equals(m, Compression.CompressionMode.Chapter)).ToList(); }
    }

    public override bool HasVolumes { get { return false; } }

    public override bool HasChapters { get { return true; } }

    public override async Task<bool> IsValid()
    {
      var baseValidation = await base.IsValid().ConfigureAwait(false);
      if (!baseValidation)
        return false;

      using (var context = Repository.GetEntityContext())
      {
        return !(await context.Get<Hentaichan>().AnyAsync(m => m.Id != Id && m.ServerName == ServerName).ConfigureAwait(false));
      }
    }

    protected override async Task CreatedFromWeb(Uri url)
    {
      await this.UpdateContent().ConfigureAwait(false);
      AddHistoryReadedUris(this.Chapters, new Uri(url.OriginalString
        .Replace("/related/", "/online/")
        .Replace("/manga/", "/online/")));

      await base.CreatedFromWeb(url).ConfigureAwait(false);
    }
  }
}
