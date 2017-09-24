using System.IO;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using System.Linq;

namespace MangaReader.Core.Convertation.Primitives
{
  public abstract class ConfigConverter : BaseConverter
  {
    protected static readonly string SettingsOldPath = Path.Combine(ConfigStorage.WorkFolder, "settings.xml");
    
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && 
        this.Version.CompareTo(NHibernate.Repository.Get<DatabaseConfig>().Single().Version) > 0 &&
        process.Version.CompareTo(this.Version) >= 0;
    }

    protected ConfigConverter()
    {
      this.Name = "Проверка настроек...";
    }
  }
}