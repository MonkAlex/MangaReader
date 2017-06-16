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

    ISiteParser Parser { get; }

    byte[] Cover { get; set; }

    #region Content

    bool HasChapters { get; set; }

    bool HasVolumes { get; set; }

    List<Volume> Volumes { get; set; }

    List<Chapter> Chapters { get; set; }

    List<MangaPage> Pages { get; set; }

    #endregion

    #region History

    IEnumerable<MangaHistory> Histories { get; }

    void AddHistory(Uri message);

    void AddHistory(IEnumerable<Uri> messages);

    void AddHistory(IEnumerable<MangaHistory> history);

    void ClearHistory();

    #endregion
    
    bool NeedUpdate { get; set; }

    bool IsValid();

    bool IsCompleted { get; set; }

    void Refresh();

    void UpdateContent();
  }
}