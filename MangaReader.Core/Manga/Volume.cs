using System.Diagnostics;
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
      get { return DatabaseConfig.GetNamingStrategy(Manga).FormateVolumeFolder(this); }
    }

    public virtual bool OnlyUpdate { get; set; }

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
