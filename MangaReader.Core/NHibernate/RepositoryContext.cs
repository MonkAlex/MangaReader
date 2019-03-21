using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Entity;
using MangaReader.Core.Exception;
using MangaReader.Core.Services;
using NHibernate;

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
    public async Task AddToTransaction<T>(T entity) where T : IEntity
    {
      var state = GetState(entity);
      await entity.BeforeSave(state).ConfigureAwait(false);
      await session.SaveOrUpdateAsync(entity).ConfigureAwait(false);
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

    public Task<object> CreateSqlQuery(string command)
    {
      return session.CreateSQLQuery(command).UniqueResultAsync();
    }

    /// <summary>
    ///  Загрузить свежую информацию из базы.
    /// </summary>
    public Task Refresh<T>(T entity) where T : IEntity
    {
      return session.RefreshAsync(entity);
    }

    /// <summary>
    /// Удалить из базы. Сохранение такой сущности создаст новую в базе.
    /// </summary>
    public async Task Delete<T>(T entity) where T : IEntity
    {
      if (entity == null || entity.Id == 0)
        return;

      using (var transaction = OpenTransaction())
      {
        var loaded = await session.LoadAsync(entity.GetType(), entity.Id).ConfigureAwait(false);
        await session.DeleteAsync(loaded).ConfigureAwait(false);
        await transaction.CommitAsync().ConfigureAwait(false);
        entity.Id = 0;
      }
    }

    /// <summary>
    /// Транзакционное сохранение с вызовом всех нужных событий.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">Сущность.</param>
    public Task Save<T>(T obj) where T : Entity.IEntity
    {
      return new[] { obj }.SaveAll(this);
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