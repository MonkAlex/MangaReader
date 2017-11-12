using System;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
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

    protected object RunSql(string command)
    {
      using (var session = Repository.GetEntityContext())
      {
        return session.CreateSqlQuery(command);
      }
    }
  }
}