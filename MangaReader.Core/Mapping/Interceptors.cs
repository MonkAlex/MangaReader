using NHibernate;
using NHibernate.Type;

namespace MangaReader.Mapping
{
  class BaseInterceptor : EmptyInterceptor
  {

    public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames,
      IType[] types)
    {
      (entity as Entity.Entity).BeforeSave(currentState, previousState, propertyNames, types);
      return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
    }
  }
}
