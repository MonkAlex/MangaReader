using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MangaReader.Core.Entity;
using MangaReader.Core.Services;

namespace MangaReader.Core.Manga
{
  public interface IManga : IDownloadable, ICompressible, IEntity, INotifyPropertyChanged
  {
    string Name { get; set; }

    string LocalName { get; set; }

    string ServerName { get; set; }

    bool IsNameChanged { get; set; }

    string Status { get; set; }

    string Description { get; set; }

    MangaSetting Setting { get; }

    ISiteParser Parser { get; }

    byte[] Cover { get; set; }

    #region Content

    bool HasChapters { get; set; }

    bool HasVolumes { get; set; }

    ICollection<Volume> Volumes { get; set; }

    ICollection<Chapter> Chapters { get; set; }

    ICollection<MangaPage> Pages { get; set; }

    #endregion

    ICollection<MangaHistory> Histories { get; }

    System.DateTime? Created { get; set; }

    bool NeedUpdate { get; set; }

    bool IsValid();

    bool IsCompleted { get; set; }

    Task Refresh();

    Task UpdateContent();

    void RefreshFolder();
  }
}