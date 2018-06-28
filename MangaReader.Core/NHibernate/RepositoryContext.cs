using System;
using System.Collections.Concurrent;
using System.Linq;
using MangaReader.Core.Entity;
using MangaReader.Core.Services;
using NHibernate;
using NHibernate.Linq;

namespace MangaReader.Core.NHibernate
{
  public class RepositoryContext : IDisposable
  {
    private ISession session;
    private int count = 0;

    private static readonly ConcurrentDictionary<ISession, RepositoryContext> Repositories = new ConcurrentDictionary<ISession, RepositoryContext>();
    private static readonly object Lock = new object();

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
        if (Repositories.TryGetValue(session, out var context))
        {
          context.count++;
          return context;
        }

        context = new RepositoryContext { session = session };
        Repositories.AddOrUpdate(session, context, (s, r) => r);
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

        try
        {
          session?.Flush();
        }
        catch (System.Exception e)
        {
          Log.Exception(e);
        }
        session?.Dispose();
        Repositories.TryRemove(session, out _);
      }
    }
  }
}