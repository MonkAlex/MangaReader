using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Services;
using NHibernate;
using NHibernate.Linq;

namespace MangaReader.Mapping
{
  public static class Repository
  {
    public static IQueryable<T> Get<T>() where T : Entity.Entity
    {
      return Environment.Session.Query<T>();
    }

    public static void Save<T>(this T obj) where T : Entity.Entity
    {
      SaveAll(new [] {obj});
    }

    public static void SaveAll<T>(this IEnumerable<T> objects) where T : Entity.Entity
    {
      var list = objects.ToList();
      if (!list.Any())
        return;

      var session = Environment.Session;
      using (var tranc = session.BeginTransaction())
      {
        try
        {
          foreach (var o in list)
          {
            Log.Add(string.Format("Save {0} with id {1}.", o.GetType().Name, o.Id));
            if (o.Id == 0)
              session.Save(o);
            else
              session.Update(o);
          }
          tranc.Commit();
        }
        catch (Exception)
        {
          tranc.Rollback();
          throw;
        }
      }
    }
  }
}