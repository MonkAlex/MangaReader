using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NHibernate.Util;

namespace MangaReader.Services
{
  static class Helper
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
    public static void Update(this IEnumerable<Entity.Entity> query)
    {
      query.ForEach(q => q.Update());
    }
    
    /// <summary>
    /// Сохранить коллекцию в базу.
    /// </summary>
    /// <param name="query"></param>
    public static void Save(this IEnumerable<Entity.Entity> query)
    {
      query.ForEach(q => q.Save());
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

  static class WindowHelper
  {
    public static Window Owner { get { return Application.Current.Windows.Cast<Window>()
          .Where(w => w.IsLoaded)
          .OrderBy(w => w.IsActive).LastOrDefault(); } }
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
