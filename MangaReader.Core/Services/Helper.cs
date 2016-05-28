using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Util;

namespace MangaReader.Core.Services
{
  public static class Helper
  {
    public static Guid TypeProperty(this Type type)
    {
      var find = type.GetProperty("Type").GetValue(null);
      return find is Guid ? (Guid)find : Guid.Empty;
    }

    public static Guid[] MangaProperty(this Type type)
    {
      var find = type.GetProperty("Manga").GetValue(null);
      return find is Guid[] ? (Guid[])find : new Guid[0];
    }

    /// <summary>
    /// Перезагрузить коллекцию из базы.
    /// </summary>
    /// <param name="query"></param>
    public static void Update(this IEnumerable<Entity.Entity> query)
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
    public static T SingleOrCreate<T>(this IQueryable<T> query) where T : Entity.Entity, new()
    {
      var single = query.SingleOrDefault();
      if (Equals(single, default(T)))
      {
        single = new T();
        single.Save();
      }

      return single;
    }

    public static List<Type> GetAllTypes<T>()
    {
      var types = new List<Type>();
      foreach (var assembly in ResolveAssembly.AllowedAssemblies())
      {
        types.AddRange(assembly.GetTypes()
          .Where(t => !t.IsAbstract && t.IsClass && typeof(T).IsAssignableFrom(t)));
      }
      return types;
    }

    public static List<T> GetEnumValues<T>()
    {
      return new List<T>(Enum.GetValues(typeof(T)).OfType<T>());
    }
  }
}
