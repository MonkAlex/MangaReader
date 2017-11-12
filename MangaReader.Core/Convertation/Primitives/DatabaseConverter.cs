using MangaReader.Core.Services.Config;
using System.Linq;

namespace MangaReader.Core.Convertation.Primitives
{
  public abstract class DatabaseConverter : BaseConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) &&
        Version.CompareTo(NHibernate.Repository.GetStateless<DatabaseConfig>().Single().Version) > 0 && 
        process.Version.CompareTo(Version) >= 0;
    }

    protected DatabaseConverter()
    {
      this.Name = "Конвертация базы данных...";
    }
  }
}