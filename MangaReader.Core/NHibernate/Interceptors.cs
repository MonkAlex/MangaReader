using System.Linq;
using MangaReader.Core.Entity;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using NHibernate;
using NHibernate.Type;

namespace MangaReader.Core.NHibernate
{
  class BaseInterceptor : EmptyInterceptor
  {

    public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames,
      IType[] types)
    {
      var nameIndex = entity is IManga ? propertyNames.ToList().IndexOf(nameof(Mangas.Name)) : -1;
      if (nameIndex > -1)
      {
        Log.Add(id != null
          ? $"Save {entity.GetType().Name} with id {id} ({currentState[nameIndex]})."
          : $"New {entity.GetType().Name} ({currentState[nameIndex]}).");
      }

      var entityImpl = entity as IEntity;
      entityImpl?.BeforeSave(new ChangeTrackerArgs(currentState, previousState, propertyNames));
      return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
    }

    public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
    {
      this.OnFlushDirty(entity, id, state, null, propertyNames, types);
      return base.OnSave(entity, id, state, propertyNames, types);
    }
  }
}
