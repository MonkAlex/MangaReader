using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using NHibernate;

namespace MangaReader.Core.Convertation.Primitives
{
  public abstract class BaseConverter
  {
    public Version Version { get; set; }

    public string Name { get; set; }

    public bool CanReportProcess { get; set; }

    public bool CanConvert(IProcess process)
    {
      return ProtectedCanConvert(process);
    }

    public async Task Convert(IProcess process)
    {
      Log.AddFormat("Converter '{0} {1}' checking...", this.Name, this.Version);
      if (this.CanConvert(process))
      {
        Log.AddFormat("Converter '{0} {1}' started", this.Name, this.Version);
        await this.ProtectedConvert(process).ConfigureAwait(false);
        Log.AddFormat("Converter '{0} {1}' completed", this.Name, this.Version);
      }
    }

    protected virtual bool ProtectedCanConvert(IProcess process)
    {
      return true;
    }

    protected abstract Task ProtectedConvert(IProcess process);

    protected bool CanConvertVersion(IProcess process)
    {
      var versionGreaterThanDatabase = this.Version.CompareTo(Repository.GetStateless<DatabaseConfig>().Single().Version) > 0;
      var versionLessOrEqualThanApp = process.Version.CompareTo(this.Version) >= 0;
      return versionGreaterThanDatabase && versionLessOrEqualThanApp;
    }

    protected async Task<object> RunSql(string command)
    {
      using (var session = Repository.GetEntityContext($"Run sql ({command.Substring(0, 50).Replace('\r', ' ').Replace('\n', ' ').Replace("   ", " ")}...)"))
      {
        return await session.CreateSqlQuery(command).ConfigureAwait(false);
      }
    }
  }
}
