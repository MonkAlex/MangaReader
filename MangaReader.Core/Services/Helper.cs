using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Entity;
using NHibernate.Util;

namespace MangaReader.Services
{
  public static class Helper
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

    /// <summary>
    /// Перезагрузить коллекцию из базы.
    /// </summary>
    /// <param name="query"></param>
    public static void Update(this IEnumerable<Entity> query)
    {
      query.ForEach(q => q.Update());
    }

    /// <summary>
    /// Перегнать байты в мегабайты.
    /// </summary>
    /// <param name="bytes">Байт.</param>
    /// <returns>Мегабайт.</returns>
    public static string ToMegaBytes(this long bytes)
    {
      return (bytes / 1024d / 1024d).ToString("0.00");
    }
  }

  public static class Generic
  {
    public static T SingleOrCreate<T>(this IQueryable<T> query) where T : Entity, new()
    {
      var single = query.SingleOrDefault();
      if (single == null)
      {
        single = new T();
        single.Save();
      }

      return single;
    }

    public static List<T> GetEnumValues<T>()
    {
      return new List<T>(Enum.GetValues(typeof(T)).OfType<T>());
    }
  }
}
