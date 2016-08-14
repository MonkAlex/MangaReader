using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Manga
{
  public class Volume : IDownloadableContainer<Chapter>
  {
    public string Name { get; set; }

    public int Number { get; set; }

    public List<Chapter> Chapters { get; set; }

    public List<Chapter> ActiveChapters { get; set; }

    public IEnumerable<Chapter> Container { get { return this.Chapters; } }

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

    public void Download(string mangaFolder)
    {
      var volumeFolder = Path.Combine(mangaFolder, this.Folder);

      this.ActiveChapters = this.Chapters;
      if (this.OnlyUpdate)
      {
        this.ActiveChapters = History.GetItemsWithoutHistory(this);
      }

      this.ActiveChapters.ForEach(c =>
      {
        c.DownloadProgressChanged += (sender, args) => this.OnDownloadProgressChanged(args);
        c.OnlyUpdate = this.OnlyUpdate;
        c.Download(volumeFolder);
      });
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
