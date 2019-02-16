using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core;
using MangaReader.Core.Services;

namespace Acomics
{
  /// <summary>
  /// Манга.
  /// </summary>
  public class Acomics : MangaReader.Core.Manga.Mangas
  {
    #region Свойства

    public override List<Compression.CompressionMode> AllowedCompressionModes
    {
      get
      {
        return base.AllowedCompressionModes
          .Where(m => this.HasVolumes && (Equals(m, Compression.CompressionMode.Volume) || Equals(m, Compression.CompressionMode.Chapter)) ||
            this.HasChapters && Equals(m, Compression.CompressionMode.Volume) ||
            Equals(m, Compression.CompressionMode.Manga))
          .ToList();
      }
    }

    protected override async Task CreatedFromWeb(Uri url)
    {
      if (this.Uri != url && Parser.ParseUri(url).Kind != UriParseKind.Manga)
      {
        await this.UpdateContent();

        var pages = this.Volumes.SelectMany(v => v.Container).SelectMany(c => c.Container)
          .Union(this.Chapters.SelectMany(c => c.Container))
          .Union(this.Pages)
          .OrderBy(p => p.Number);
        AddHistoryReadedUris(pages, url);
      }

      await base.CreatedFromWeb(url);
    }

    #endregion

  }
}
