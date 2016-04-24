using System.Collections.Generic;
using System.Linq;
using MangaReader.Services;
using NHibernate.Linq;

namespace MangaReader.Core.NHibernate
{
  public static class Repository
  {
    public static IQueryable<T> Get<T>() where T : Entity.Entity
    {
      return Mapping.Session.Query<T>();
    }

    public static void Save<T>(T obj) where T : Entity.Entity
    {
      SaveAll(new [] {obj});
    }

    public static void SaveAll<T>(this IEnumerable<T> objects) where T : Entity.Entity
    {
      var list = objects.ToList();
      if (!list.Any())
        return;

      var session = Mapping.Session;
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