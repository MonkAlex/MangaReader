using System;
using System.Threading;
using System.Threading.Tasks;
using MangaReader.Core.Entity;
using MangaReader.Core.Exception;
using NHibernate.Event;

namespace MangaReader.Core.NHibernate
{
  class EntityChangedEvent : IPreUpdateEventListener, IPreInsertEventListener, IPreDeleteEventListener
  {
    public async Task<bool> OnPreUpdateAsync(PreUpdateEvent e, CancellationToken cancellationToken)
    {
      if (e.Entity is IEntity entity)
      {
        if (e.OldState == null && entity.Id != 0)
          throw new EntityException<IEntity>("Сущности можно обновлять только в контексте подключения к БД (получение, изменение, сохранение).", entity);
        await entity.BeforeSave(new ChangeTrackerArgs(e.State, e.OldState, e.Persister.PropertyNames, false)).ConfigureAwait(false);
      }
      return false;
    }

    public bool OnPreUpdate(PreUpdateEvent e)
    {
      throw new NotImplementedException();
    }

    public async Task<bool> OnPreInsertAsync(PreInsertEvent e, CancellationToken cancellationToken)
    {
      if (e.Entity is IEntity entity)
        await entity.BeforeSave(new ChangeTrackerArgs(e.State, null, e.Persister.PropertyNames, false)).ConfigureAwait(false);
      return false;
    }

    public bool OnPreInsert(PreInsertEvent e)
    {
      throw new NotImplementedException();
    }

    public async Task<bool> OnPreDeleteAsync(PreDeleteEvent e, CancellationToken cancellationToken)
    {
      if (e.Entity is IEntity entity)
        await entity.BeforeDelete(new ChangeTrackerArgs(e.DeletedState, null, e.Persister.PropertyNames, false)).ConfigureAwait(false);
      return false;
    }

    public bool OnPreDelete(PreDeleteEvent e)
    {
      throw new NotImplementedException();
    }
  }
}
