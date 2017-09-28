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
    public string Name { get; set; }

    public int Number { get; set; }

    public override IEnumerable<Chapter> InDownloading { get; protected set; }

    public string Folder
    {
      get { return this.folderPrefix + this.Number.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0'); }
      private set { this.folderPrefix = value; }
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
