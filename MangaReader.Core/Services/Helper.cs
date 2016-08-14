using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;
using NHibernate.Util;

namespace MangaReader.Core.Services
{
  public static class Helper
  {
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

    /// <summary>
    /// Получить сборки, которые относятся к приложению.
    /// </summary>
    /// <returns>Сборки.</returns>
    /// <remarks>В текущей реализации - по namespace.</remarks>
    public static List<Assembly> AllowedAssemblies()
    {
      var byName = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("MangaReader")).ToList();
      byName.AddRange(ConfigStorage.Plugins.Select(p => p.Assembly));
      byName = byName.Distinct().ToList();
      return byName;
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
      foreach (var assembly in Helper.AllowedAssemblies())
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

    public static Uri GetLoginMainUri<T>() where T : IManga
    {
      if (NHibernate.Mapping.Initialized)
      {
        var plugin = ConfigStorage.GetPlugin<T>();
        if (plugin != null)
        {
          var login = plugin.GetLogin();
          if (login != null)
            return login.MainUri;
        }
      }

      return null;
    }
  }
}
