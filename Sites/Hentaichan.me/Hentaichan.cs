using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;

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

    public override bool IsValid()
    {
      var baseValidation = base.IsValid();
      if (!baseValidation)
        return false;

      using (var context = Repository.GetEntityContext())
      {
        return !context.Get<Hentaichan>().Any(m => m.Id != Id && m.ServerName == ServerName);
      }
    }

    protected override void CreatedFromWeb(Uri url)
    {
      this.UpdateContent();
      AddHistoryReadedUris(this.Chapters, new Uri(url.OriginalString
        .Replace("/related/", "/manga/")
        .Replace("/online/", "/manga/")));

      base.CreatedFromWeb(url);
    }
  }
}
