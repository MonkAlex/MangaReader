using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.NHibernate
{
  public static class Repository
  {
    public static RepositoryContext GetEntityContext()
    {
      return RepositoryContext.Create(Guid.NewGuid().ToString("D"));
    }

    public static RepositoryContext GetEntityContext(string name)
    {
      return RepositoryContext.Create(name);
    }

    public static T GetStateless<T>(int id) where T : Entity.IEntity
    {
      using (var session = Mapping.GetStatelessSession())
      {
        return session.Get<T>(id);
      }
    }

    public static List<T> GetStateless<T>() where T : Entity.IEntity
    {
      using (var session = Mapping.GetStatelessSession())
      {
        return session.Query<T>().ToList();
      }
    }

    public static Guid GetDatabaseUniqueId()
    {
      using (var context = GetEntityContext("Get database unique id"))
      {
        var id = context.Get<DatabaseConfig>().Single().UniqueId;
        Log.Add($"Database unique id = {id:D}");
        return id;
      }
    }
  }
}