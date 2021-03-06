﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using MangaReader.Core.Services.Config;
using System.Threading.Tasks;
using MangaReader.Core.NHibernate;

namespace MangaReader.Core.Services
{
  public static class Helper
  {

    public static readonly Regex SpaceRegex = new Regex(@"\s{2,}", RegexOptions.Compiled);

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

      return HumanizeByteSize((long)byteCount);
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

    public static void StartUseShell(string uri)
    {
      Log.Add($"Try to start '{uri}'");
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      {
        Process.Start("xdg-open", $"\"{uri}\"");
      }
      else
      {
        var psi = new ProcessStartInfo(uri) { UseShellExecute = true };
        Process.Start(psi);
      }
    }

  }

  public static class Generic
  {
    public static Guid GetNamingStrategyId<T>() where T : IFolderNamingStrategy
    {
      var obj = Activator.CreateInstance<T>();
      return obj.Id;
    }

    public static async Task<T> SingleOrCreate<T>(this IEnumerable<T> query) where T : Entity.Entity, new()
    {
      var single = query.SingleOrDefault();
      if (Equals(single, default(T)))
      {
        using (var context = Repository.GetEntityContext())
        {
          single = new T();
          await context.Save(single).ConfigureAwait(false);
        }
      }

      return single;
    }

    public static List<Type> GetAllTypes<T>()
    {
      var types = new List<Type>();
      foreach (var assembly in Helper.AllowedAssemblies())
      {
        types.AddRange(assembly.GetTypes()
          .Where(t => !t.IsAbstract
                      && t.IsClass
                      && typeof(T).IsAssignableFrom(t)));
      }
      return types;
    }

    public static List<Type> GetAllPublicTypes<T>()
    {
      return GetAllTypes<T>().Where(t => t.IsPublic).ToList();
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

    public static async Task SaveAll<T>(this IEnumerable<T> objects, RepositoryContext context) where T : Entity.IEntity
    {
      var list = objects.ToList();
      if (!list.Any())
        return;

      using (var transaction = context.OpenTransaction())
      {
        try
        {
          foreach (var o in list)
            await context.AddToTransaction(o).ConfigureAwait(false);
          await transaction.CommitAsync().ConfigureAwait(false);
        }
        catch (System.Exception)
        {
          await transaction.RollbackAsync().ConfigureAwait(false);
          throw;
        }
      }
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

    public static IAsyncEnumerable<TR> SelectAsync<T, TR>(this IEnumerable<T> seq, Func<T, Task<TR>> selector)
    {
      return AsyncEnumerable.CreateEnumerable(() =>
      {
        IEnumerator<T> seqEnum = seq.GetEnumerator();
        var current = default(TR);
        return AsyncEnumerable.CreateEnumerator(
          moveNext: async ct =>
          {
            if (!seqEnum.MoveNext())
              return false;
            current = await selector(seqEnum.Current).ConfigureAwait(false);
            return true;
          },
          current: () => current,
          dispose: seqEnum.Dispose);
      });
    }

    public static Task<List<T>> ToListAsync<T>(this IQueryable<T> queryable)
    {
      return global::NHibernate.Linq.LinqExtensionMethods.ToListAsync(queryable);
    }

    public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate)
    {
      return global::NHibernate.Linq.LinqExtensionMethods.FirstOrDefaultAsync(queryable, predicate);
    }

    public static Task<T> FirstAsync<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate)
    {
      return global::NHibernate.Linq.LinqExtensionMethods.FirstAsync(queryable, predicate);
    }

    public static Task<T> SingleAsync<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate)
    {
      return global::NHibernate.Linq.LinqExtensionMethods.SingleAsync(queryable, predicate);
    }

    public static Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate)
    {
      return global::NHibernate.Linq.LinqExtensionMethods.SingleOrDefaultAsync(queryable, predicate);
    }

    public static Task<T> SingleAsync<T>(this IQueryable<T> queryable)
    {
      return global::NHibernate.Linq.LinqExtensionMethods.SingleAsync(queryable);
    }

    public static Task<bool> AnyAsync<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate)
    {
      return global::NHibernate.Linq.LinqExtensionMethods.AnyAsync(queryable, predicate);
    }

    public static Task<bool> AnyAsync<T>(this IQueryable<T> queryable)
    {
      return global::NHibernate.Linq.LinqExtensionMethods.AnyAsync(queryable);
    }
  }
}
