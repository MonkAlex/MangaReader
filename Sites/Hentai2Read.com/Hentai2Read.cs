using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core;
using MangaReader.Core.Services;

namespace Hentai2Read.com
{
  /// <summary>
  /// Манга.
  /// </summary>
  public class Hentai2Read : MangaReader.Core.Manga.Mangas
  {
    public override List<Compression.CompressionMode> AllowedCompressionModes
    {
      get { return base.AllowedCompressionModes.Where(m => !Equals(m, Compression.CompressionMode.Chapter)).ToList(); }
    }

    public override bool HasVolumes { get { return false; } }

    public override bool HasChapters { get { return true; } }

    protected override void CreatedFromWeb(Uri url)
    {
      base.CreatedFromWeb(url);

      if (this.Uri != url && Parser.ParseUri(url).Kind != UriParseKind.Manga)
      {
        this.UpdateContent();

        AddHistoryReadedUris(this.Chapters, url);
      }
    }
  }
}
