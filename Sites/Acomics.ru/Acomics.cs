﻿using System;
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

    protected override void Created(Uri url)
    {
      if (this.Uri != url)
      {
        this.UpdateContent();

        var pages = this.Volumes.SelectMany(v => v.Chapters).SelectMany(c => c.Pages)
          .Union(this.Chapters.SelectMany(c => c.Pages))
          .Union(this.Pages)
          .OrderBy(p => p.Number);
        AddHistoryReadedUris(pages, url);
      }

      base.Created(url);
    }

    #endregion

  }
}