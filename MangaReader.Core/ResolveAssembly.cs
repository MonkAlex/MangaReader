using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MangaReader.Services;

namespace MangaReader.Core
{
  internal static class ResolveAssembly
  {
    internal static Assembly ResolveInternalAssembly(object sender, ResolveEventArgs args)
    {
      return FindResource(args.Name);
    }

    private static Assembly FindResource(string resource)
    {
      foreach (var assembly in AllowedAssemblies())
      {
        var resourceNames = assembly.GetManifestResourceNames();
        if (!resourceNames.Any())
          continue;

        if (resource.Contains(','))
          resource = resource.Substring(0, resource.IndexOf(','));

        var assemblyFile = string.Format(".{0}.dll", resource);
        var resourceName = resourceNames.SingleOrDefault(s => s.EndsWith(assemblyFile));
        if (string.IsNullOrWhiteSpace(resourceName))
        {
          assemblyFile = assemblyFile.Remove(0, 1);
          resourceName = resourceNames.SingleOrDefault(s => s.EndsWith(assemblyFile));
        }

        if (string.IsNullOrWhiteSpace(resourceName))
          continue;

        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
          var block = new byte[stream.Length];
          stream.Read(block, 0, block.Length);
          return Assembly.Load(block);
        }
      }
      return null;
    }

    private static List<Assembly> AllowedAssemblies()
    {
      return AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("MangaReader")).ToList();
    }

    /// <summary>
    /// Загрузка SQLite.
    /// </summary>
    /// <remarks>
    /// Используется 
    /// Precompiled Binaries for 32-bit Windows (.NET Framework 4.5) 
    /// с адреса 
    /// https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki
    /// </remarks>
    internal static void LoadSql()
    {
      foreach (var assembly in AllowedAssemblies())
      {
        var resourceNames = assembly.GetManifestResourceNames();
        if (!resourceNames.Any())
          continue;

        var assemblyFile = "System.Data.SQLite.dll";
        var resourceName = resourceNames.SingleOrDefault(s => s.EndsWith(assemblyFile));
        if (string.IsNullOrWhiteSpace(resourceName))
          continue;

        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
          var block = new byte[stream.Length];
          stream.Read(block, 0, block.Length);
          var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), assemblyFile);
          File.WriteAllBytes(path, block);
          Assembly.LoadFrom(path);
          Log.Add(string.Format("Assembly {0} loaded from {1}.", assemblyFile, path));
        }
      }
    }

  }
}