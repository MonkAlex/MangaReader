using MangaReader.Core.Entity;
using NHibernate.Event;

namespace MangaReader.Core.NHibernate
{
  class SaveOrUpdateEvent : IPreUpdateEventListener, IPreInsertEventListener
  {
    public bool OnPreUpdate(PreUpdateEvent e)
    {
      if (e.Entity is IEntity entity)
        entity.BeforeSave(new ChangeTrackerArgs(e.State, e.OldState, e.Persister.PropertyNames));
      return false;
    }

    public bool OnPreInsert(PreInsertEvent e)
    {
      if (e.Entity is IEntity entity)
        entity.BeforeSave(new ChangeTrackerArgs(e.State, null, e.Persister.PropertyNames));
      return false;
    }
  }
}
