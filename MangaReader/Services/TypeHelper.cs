using System;
using System.Linq;

namespace MangaReader.Services
{
  static class TypeHelper
  {
    public static Guid TypeProperty(this Type type)
    {
      var find = type.GetProperty("Type").GetValue(null);
      return find is Guid ? (Guid)find : Guid.Empty;
    }

    public static Guid MangaProperty(this Type type)
    {
      var find = type.GetProperty("Manga").GetValue(null);
      return find is Guid ? (Guid)find : Guid.Empty;
    }
  }

  static class Generic
  {
    public static T SingleOrCreate<T>(this IQueryable<T> query) where T : Entity.Entity, new()
    {
      var single = query.SingleOrDefault();
      if (single == null)
      {
        single = new T();
        single.Save();
      }

      return single;
    }
  }
}
