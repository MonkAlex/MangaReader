using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Services;

namespace MangaReader.Core.Manga
{
  [DebuggerDisplay("{Number} {Name}")]
  public class Volume : DownloadableContainerImpl<Chapter>
  {
    public int Number { get; set; }

    public override string Folder
    {
      get { return FolderNamingStrategies.GetNamingStrategy(Manga).FormateVolumeFolder(this); }
    }

    public virtual bool OnlyUpdate { get; set; }

    public override Task Download(string mangaFolder = null)
    {
      if (!DirectoryHelpers.ValidateSettingPath(mangaFolder))
        throw new DirectoryNotFoundException($"Попытка скачивания в папку {mangaFolder}, папка не существует.");
      var volumeFolder = Path.Combine(mangaFolder, DirectoryHelpers.RemoveInvalidCharsFromName(this.Folder));
      if (!Directory.Exists(volumeFolder))
        Directory.CreateDirectory(volumeFolder);

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
