using System.Collections.Generic;
using MangaReader.Core.Services;

namespace MangaReader.Core.Manga
{
  public interface ICompressible
  {
    void Compress();

    bool? NeedCompress { get; set; }

    List<Compression.CompressionMode> AllowedCompressionModes { get; }

    Compression.CompressionMode? CompressionMode { get; set; }
  }
}