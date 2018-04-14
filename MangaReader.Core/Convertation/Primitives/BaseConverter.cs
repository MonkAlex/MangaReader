using System;
using System.Linq;
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

    public void Convert(IProcess process)
    {
      if (this.CanConvert(process))
      {
        Log.AddFormat("Converter '{0}{1}' started", this.Name, this.Version);
        this.ProtectedConvert(process);
        Log.AddFormat("Converter '{0}{1}' completed", this.Name, this.Version);
      }
    }

    protected virtual bool ProtectedCanConvert(IProcess process)
    {
      return true;
    }

    protected virtual void ProtectedConvert(IProcess process)
    {

    }

    protected bool CanConvertVersion(IProcess process)
    {
      return this.Version.CompareTo(Repository.GetStateless<DatabaseConfig>().Single().Version) > 0 &&
             process.Version.CompareTo(this.Version) >= 0;
    }

    protected object RunSql(string command)
    {
      using (var session = Repository.GetEntityContext())
      {
        return session.CreateSqlQuery(command);
      }
    }
  }
}