using System.IO;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation.Primitives
{
  public abstract class ConfigConverter : BaseConverter
  {
    protected static readonly string SettingsOldPath = Path.Combine(ConfigStorage.WorkFolder, "settings.xml");
    
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && this.CanConvertVersion(process);
    }

    protected ConfigConverter()
    {
      this.Name = "Проверка настроек...";
    }
  }
}