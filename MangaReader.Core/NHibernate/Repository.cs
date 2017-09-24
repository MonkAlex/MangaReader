using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Services;
using NHibernate.Linq;

namespace MangaReader.Core.NHibernate
{
  public static class Repository
  {
    public static IQueryable<T> Get<T>() where T : Entity.IEntity
    {
      return Mapping.GetStatelessSession().Query<T>();
    }

    public static T Get<T>(int id) where T : Entity.IEntity
    {
      using (var session = Mapping.GetSession())
      {
        return session.Get<T>(id);
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

      using (var session = Mapping.GetSession())
      {
        using (var tranc = session.BeginTransaction())
        {
          try
          {
            foreach (var o in list)
            {
              Log.AddFormat("Save {0} with id {1}.", o.GetType().Name, o.Id);
              if (o.Id == 0)
                session.Save(o);
              else
                session.Update(o);
            }
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