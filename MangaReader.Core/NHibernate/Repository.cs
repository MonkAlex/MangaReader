using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

    public static List<T> GetStateless<T>(Expression<Func<T, bool>> where) where T : Entity.IEntity
    {
      using (var session = Mapping.GetStatelessSession())
      {
        return session.Query<T>().Where(where).ToList();
      }
    }

    public static Guid GetDatabaseUniqueId()
    {
      using (var context = GetEntityContext())
      {
        return context.Get<DatabaseConfig>().Single().UniqueId;
      }
    }

    public static void Save<T>(T obj) where T : Entity.IEntity
    {
      SaveAll(new[] {obj});
    }
    
    public static void SaveAll<T>(this IEnumerable<T> objects) where T : Entity.IEntity
    {
      var list = objects.ToList();
      if (!list.Any())
        return;

      using (var context = Repository.GetEntityContext())
      {
        using (var tranc = context.OpenTransaction())
        {
          try
          {
            foreach (var o in list)
              context.SaveOrUpdate(o);
            tranc.Commit();
          }
          catch (System.Exception)
          {
            tranc.Rollback();
            throw;
          }
        }
      }
    }
  }
}