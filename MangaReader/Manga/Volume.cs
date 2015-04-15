using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MangaReader.Services;

namespace MangaReader.Manga
{
  public class Volume : IDownloadable
  {
    public string Name { get; set; }

    public int Number { get; set; }

    public List<Chapter> Chapters { get; set; }

    public List<Chapter> ActiveChapters { get; set; }

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


    public string Folder
    {
      get { return this.folderPrefix + this.Number.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0'); }
      private set { this.folderPrefix = value; }
    }

    private string folderPrefix = Settings.VolumePrefix;

    public event EventHandler<Mangas> DownloadProgressChanged;

    protected void OnDownloadProgressChanged(Mangas e)
    {
      var handler = DownloadProgressChanged;
      if (handler != null)
        handler(this, e);
    }

    public void Download(string mangaFolder)
    {
      var volumeFolder = Path.Combine(mangaFolder, this.Folder);

      this.ActiveChapters = this.Chapters;
      if (Settings.Update)
      {
        this.ActiveChapters = History.GetNotSavedChapters(this.ActiveChapters);
      }

      this.ActiveChapters.ForEach(c =>
      {
        c.DownloadProgressChanged += (sender, args) => this.OnDownloadProgressChanged(args);
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
