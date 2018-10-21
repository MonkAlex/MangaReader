using System.Threading;
using System.Threading.Tasks;
using MangaReader.Core.Entity;
using MangaReader.Core.Exception;
using NHibernate.Event;

namespace MangaReader.Core.NHibernate
{
  class SaveOrUpdateEvent : IPreUpdateEventListener, IPreInsertEventListener
  {
    public Task<bool> OnPreUpdateAsync(PreUpdateEvent e, CancellationToken cancellationToken)
    {
      return Task.FromResult(this.OnPreUpdate(e));
    }

    public bool OnPreUpdate(PreUpdateEvent e)
    {
      if (e.Entity is IEntity entity)
      {
        if (e.OldState == null && entity.Id != 0)
          throw new EntityException("Сущности можно обновлять только в контексте подключения к БД (получение, изменение, сохранение).", entity);
        entity.BeforeSave(new ChangeTrackerArgs(e.State, e.OldState, e.Persister.PropertyNames, false));
      }
      return false;
    }

    public Task<bool> OnPreInsertAsync(PreInsertEvent e, CancellationToken cancellationToken)
    {
      return Task.FromResult(this.OnPreInsert(e));
    }

    public bool OnPreInsert(PreInsertEvent e)
    {
      if (e.Entity is IEntity entity)
        entity.BeforeSave(new ChangeTrackerArgs(e.State, null, e.Persister.PropertyNames, false));
      return false;
    }
  }
}
