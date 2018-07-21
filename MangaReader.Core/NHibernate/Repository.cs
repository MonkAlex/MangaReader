using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Services.Config;
using NHibernate.Linq;

namespace MangaReader.Core.NHibernate
{
  public static class Repository
  {
    public static RepositoryContext GetEntityContext()
    {
      return RepositoryContext.Create();
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
      using (var context = GetEntityContext())
      {
        return context.Get<DatabaseConfig>().Single().UniqueId;
      }
    }
  }
}