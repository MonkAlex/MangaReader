using System;
using System.Collections.Generic;
using MangaReader.Core.Entity;
using MangaReader.Core.Services;

namespace MangaReader.Core.Manga
{
  public interface IManga : IDownloadable, ICompressible, IEntity
  {
    string Name { get; set; }

    string LocalName { get; set; }

    string ServerName { get; set; }

    bool IsNameChanged { get; set; }

    string Status { get; set; }

    MangaSetting Setting { get; }

    IEnumerable<MangaHistory> Histories { get; }

    void AddHistory(Uri message);

    void AddHistory(IEnumerable<Uri> messages);

    void AddHistory(IEnumerable<MangaHistory> history);

    void ClearHistory();

    bool NeedUpdate { get; set; }

    bool IsValid();

    bool IsCompleted { get; set; }

    void Refresh();
  }
}