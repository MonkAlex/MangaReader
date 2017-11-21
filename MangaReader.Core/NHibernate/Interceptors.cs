using System.Linq;
using System.Reflection;
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
      var nameIndex = propertyNames.ToList().IndexOf(nameof(Mangas.ServerName));
      if (nameIndex > 0)
      {
        Log.Add(id != null
          ? $"Save {entity.GetType().Name} with id {id} ({currentState[nameIndex]})."
          : $"New {entity.GetType().Name} ({currentState[nameIndex]}).");
      }
      var method = entity.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).SingleOrDefault(m => m.Name == "BeforeSave");
      if (method != null)
      {
        var parameters = method.GetParameters();
        if (parameters.Length == 3 &&
            parameters[0].ParameterType == typeof(object[]) &&
            parameters[1].ParameterType == typeof(object[]) &&
            parameters[2].ParameterType == typeof(string[]))
        {
          method.Invoke(entity, new object[] { currentState, previousState, propertyNames });
        }
        else
        {
          throw new Exception.SaveValidationException("Method BeforeSave(object[] currentState, object[] previousState, string[] propertyNames) not found.", (Entity.Entity)entity);
        }
      }
      else
      {
        throw new Exception.SaveValidationException("Method BeforeSave not found.", (Entity.Entity)entity);
      }
      return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
    }

    public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
    {
      this.OnFlushDirty(entity, id, state, null, propertyNames, types);
      return base.OnSave(entity, id, state, propertyNames, types);
    }
  }
}
