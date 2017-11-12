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
  public class Volume : DownloadableContainerImpl<Chapter>
  {
    public int Number { get; set; }

    public override string Folder
    {
      get { return this.folderPrefix + this.Number.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0'); }
    }

    public virtual bool OnlyUpdate { get; set; }

    private string folderPrefix = AppConfig.VolumePrefix;

    public override Task Download(string mangaFolder = null)
    {
      var volumeFolder = Path.Combine(mangaFolder, this.Folder);

      this.InDownloading = this.Container.ToList();
      if (this.OnlyUpdate)
      {
        this.InDownloading = History.GetItemsWithoutHistory(this);
      }

      var tasks = this.InDownloading.Select(c =>
      {
        c.OnlyUpdate = this.OnlyUpdate;
        return c.Download(volumeFolder);
      });
      return Task.WhenAll(tasks);
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

    }
  }
}
