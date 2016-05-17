using System;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;

namespace MangaReader.Core.Convertation.Primitives
{
  public abstract class BaseConverter
  {
    public Version Version { get; set; }

    public bool CanConvert(IProcess process)
    {
      return ProtectedCanConvert(process);
    }

    public void Convert(IProcess process)
    {
      if (this.CanConvert(process))
        this.ProtectedConvert(process);
    }

    protected virtual bool ProtectedCanConvert(IProcess process)
    {
      return true;
    }

    protected virtual void ProtectedConvert(IProcess process)
    {
      
    }

    protected void RunSql(string command)
    {
      var query = Mapping.Session.CreateSQLQuery(command);
      query.UniqueResult();
    }

  }
}