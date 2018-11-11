using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MangaReader.Core.Entity;
using MangaReader.Core.Exception;
using MangaReader.Core.Services;
using NHibernate;
using NHibernate.Linq;

namespace MangaReader.Core.NHibernate
{
  public class RepositoryContext : IDisposable
  {
    private ISession session;
    private int count = 0;
    private Stopwatch stopwatch;

    private static readonly ConcurrentDictionary<ISession, RepositoryContext> Repositories = new ConcurrentDictionary<ISession, RepositoryContext>();
    private static readonly object Lock = new object();

    public string Name { get; set; }

    public IQueryable<T> Get<T>() where T : IEntity
    {
      return session.Query<T>();
    }

    /// <summary>
    /// Добавление сущности в транзакцию.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity">Сущность.</param>
    public void AddToTransaction<T>(T entity) where T : IEntity
    {
      var state = GetState(entity);
      entity.BeforeSave(state);
      session.SaveOrUpdate(entity);
    }

    public ITransaction OpenTransaction()
    {
      return session.BeginTransaction();
    }

    /// <summary>
    /// Получить изменения сущности в рамках сессии.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity">Сущность.</param>
    /// <returns>Изменения.</returns>
    public ChangeTrackerArgs GetState<T>(T entity) where T : IEntity
    {
      var impl = session.GetSessionImplementation();
      var key = impl.PersistenceContext.GetEntry(entity);

      if (key?.LoadedState == null && entity.Id != 0)
        throw new EntityException("Сущности можно обновлять только в контексте подключения к БД (получение, изменение, сохранение).", entity);

      if (key == null)
      {
        var name = impl.GuessEntityName(entity);
        var persister = impl.GetEntityPersister(name, entity);
        return new ChangeTrackerArgs(persister.GetPropertyValues(entity), null, persister.PropertyNames, true);
      }

      var current = key.Persister.GetPropertyValues(entity);
      return new ChangeTrackerArgs(current, key.LoadedState, key.Persister.PropertyNames, true);
    }

    public static RepositoryContext Create(string name)
    {
      lock (Lock)
      {
        var session = Mapping.GetSession();
        if (Repositories.TryGetValue(session, out var context))
        {
          context.count++;
          return context;
        }

        context = new RepositoryContext { session = session, Name = name };
        Repositories.AddOrUpdate(session, context, (s, r) => r);
        context.stopwatch = new Stopwatch();
        context.stopwatch.Start();
        return context;
      }
    }

    public object CreateSqlQuery(string command)
    {
      return session.CreateSQLQuery(command).UniqueResult();
    }

    /// <summary>
    ///  Загрузить свежую информацию из базы.
    /// </summary>
    public void Refresh<T>(T entity) where T : IEntity
    {
      session.Refresh(entity);
    }

    /// <summary>
    /// Удалить из базы. Сохранение такой сущности создаст новую в базе.
    /// </summary>
    public void Delete<T>(T entity) where T : IEntity
    {
      if (entity == null || entity.Id == 0)
        return;

      using (var tranc = OpenTransaction())
      {
        var loaded = session.Load(entity.GetType(), entity.Id);
        session.Delete(loaded);
        tranc.Commit();
        entity.Id = 0;
      }
    }

    /// <summary>
    /// Транзакционное сохранение с вызовом всех нужных событий.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">Сущность.</param>
    public void Save<T>(T obj) where T : Entity.IEntity
    {
      new[] { obj }.SaveAll(this);
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

        stopwatch.Stop();
        Log.Add($"Context {Name} closed (Elapsed {stopwatch.Elapsed.TotalSeconds:F2} seconds)");
        session?.Dispose();
        Repositories.TryRemove(session, out _);
      }
    }
  }
}