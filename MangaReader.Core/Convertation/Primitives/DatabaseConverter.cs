using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation.Primitives
{
  public abstract class DatabaseConverter : BaseConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) &&
        Version.CompareTo(ConfigStorage.Instance.DatabaseConfig.Version) > 0 && 
        process.Version.CompareTo(Version) >= 0;
    }
  }
}