using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MangaReader.Core.Entity;
using MangaReader.Core.Services.Config;
using NHibernate;
using NHibernate.Linq;

namespace MangaReader.Core.NHibernate
{
  public class RepositoryContext : IDisposable
  {
    private ISession session;
    private int count = 0;

    private static readonly ConcurrentDictionary<ISession, RepositoryContext> repositories = new ConcurrentDictionary<ISession, RepositoryContext>();
    private static object Lock = new object();

    public IQueryable<T> Get<T>() where T : IEntity
    {
      return session.Query<T>();
    }

    public void SaveOrUpdate<T>(T entity) where T : IEntity
    {
      session.SaveOrUpdate(entity);
    }

    public ITransaction OpenTransaction()
    {
      return session.BeginTransaction();
    }

    public static RepositoryContext Create()
    {
      lock (Lock)
      {
        var session = Mapping.GetSession();
        if (repositories.TryGetValue(session, out var context))
        {
          context.count++;
          return context;
        }

        context = new RepositoryContext { session = session };
        repositories.AddOrUpdate(session, context, (s, r) => r);
        return context;
      }
    }

    public object CreateSqlQuery(string command)
    {
      return session.CreateSQLQuery(command).UniqueResult();
    }

    public void Refresh<T>(T entity) where T : IEntity
    {
      session.Refresh(entity);
    }

    public void Delete<T>(T entity) where T : IEntity
    {
      var loaded = session.Load(entity.GetType(), entity.Id);
      session.Delete(loaded);
    }

    public void Dispose()
    {
      lock (Lock)
      {
        if (count != 0)
        {
          count--;
          return;
        }
        
        repositories.TryRemove(session, out _);
        session?.Dispose();
      }
    }
  }

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