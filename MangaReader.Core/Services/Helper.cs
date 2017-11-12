using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;
using NHibernate.Util;
using System.Threading.Tasks;

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

    public static string HumanizeByteSize(this long byteCount)
    {
      string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
      if (byteCount == 0)
        return "0" + suf[0];
      long bytes = Math.Abs(byteCount);
      int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
      double num = Math.Round(bytes / Math.Pow(1024, place), 1);
      return Math.Sign(byteCount) * num + suf[place];
    }

    public static string HumanizeByteSize(this double byteCount)
    {
      if (double.IsNaN(byteCount) || double.IsInfinity(byteCount) || byteCount == 0)
        return string.Empty;

      return HumanizeByteSize((long) byteCount);
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
    public static T SingleOrCreate<T>(this IEnumerable<T> query) where T : Entity.Entity, new()
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

    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
      if (collection == null || collection.IsReadOnly || items == null)
        return;

      foreach (var item in items)
        collection.Add(item);
    }

    public static Uri GetLoginMainUri<T>() where T : IManga
    {
      if (NHibernate.Mapping.Initialized)
      {
        var plugin = ConfigStorage.GetPlugin<T>();
        if (plugin != null)
        {
          var login = plugin.GetSettings();
          if (login != null)
            return login.Login.MainUri;
        }
      }

      return null;
    }

    public static Task LogException(this Task task)
    {
      return LogException(task, string.Empty, string.Empty);
    }

    public static Task LogException(this Task task, string onsuccess, string onfail)
    {
      return task.ContinueWith(t =>
      {
        if (t.Exception != null)
          Log.Exception(t.Exception, onfail);
        else if (!string.IsNullOrWhiteSpace(onsuccess))
          Log.Info(onsuccess);
      });
    }
  }
}
