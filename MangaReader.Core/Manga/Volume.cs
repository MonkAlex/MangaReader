using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Manga
{
  [DebuggerDisplay("{Number} {Name}")]
  public class Volume : Entity.Entity, IDownloadableContainer<Chapter>
  {
    public string Name { get; set; }

    public int Number { get; set; }

    public List<Chapter> Chapters { get; set; }

    public List<Chapter> ActiveChapters { get; set; }

    public IEnumerable<Chapter> Container
    {
      get { return this.Chapters; }
      protected set { RefreshPagesList(value); }
    }

    /// <summary>
    /// Статус загрузки.
    /// </summary>
    public virtual bool IsDownloaded
    {
      get { return this.ActiveChapters != null && this.ActiveChapters.Any() && this.ActiveChapters.All(v => v.IsDownloaded); }
    }

    /// <summary>
    /// Процент загрузки манги.
    /// </summary>
    public virtual double Downloaded
    {
      get { return (this.ActiveChapters != null && this.ActiveChapters.Any()) ? this.ActiveChapters.Average(ch => ch.Downloaded) : 0; }
      set { }
    }

    public Uri Uri { get; set; }
    
    public string Folder
    {
      get { return this.folderPrefix + this.Number.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0'); }
      private set { this.folderPrefix = value; }
    }

    public virtual bool OnlyUpdate { get; set; }

    private string folderPrefix = AppConfig.VolumePrefix;

    public event EventHandler<IManga> DownloadProgressChanged;

    protected void OnDownloadProgressChanged(IManga e)
    {
      var handler = DownloadProgressChanged;
      if (handler != null)
        handler(this, e);
    }

    public Task Download(string mangaFolder)
    {
      var volumeFolder = Path.Combine(mangaFolder, this.Folder);

      this.ActiveChapters = this.Chapters.ToList();
      if (this.OnlyUpdate)
      {
        this.ActiveChapters = History.GetItemsWithoutHistory(this);
      }

      var tasks = this.ActiveChapters.Select(c =>
      {
        c.OnlyUpdate = this.OnlyUpdate;
        return c.Download(volumeFolder);
      });
      return Task.WhenAll(tasks);
    }
    
    protected virtual void RefreshPagesList(IEnumerable<Chapter> chapters)
    {
      if (chapters != this.Chapters)
      {
        this.Chapters.ForEach(p => p.DownloadProgressChanged -= OnDownloadProgressChanged);
        this.Chapters = chapters.ToList();
      }
      this.Chapters.ForEach(p => p.DownloadProgressChanged += OnDownloadProgressChanged);
    }
    
    private void OnDownloadProgressChanged(object sender, IManga manga)
    {
      this.OnDownloadProgressChanged(manga);
    }

    public Volume(string name, int number)
      : this(number)
    {
      this.Name = name;
    }

    public Volume(int number)
      : this()
    {
      this.Number = number;
    }

    public Volume()
    {
      this.Chapters = new List<Chapter>();
    }
  }
}
