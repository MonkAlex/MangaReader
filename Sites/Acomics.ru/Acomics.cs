using System;
using System.Collections.Generic;
using System.Linq;
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

    #endregion

  }
}
