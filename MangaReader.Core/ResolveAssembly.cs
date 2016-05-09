using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

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

    internal static List<Assembly> AllowedAssemblies()
    {
      return AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("MangaReader")).ToList();
    }

    /// <summary>
    /// Загрузка SQLite.
    /// </summary>
    internal static void LoadSql()
    {
      foreach (var assembly in AllowedAssemblies())
      {
        WriteResourceToFile(assembly, "System.Data.SQLite.dll");
        WriteResourceToFile(assembly, "sqlite3");
        WriteResourceToFile(assembly, "sqlite3.dll", "x64");
        WriteResourceToFile(assembly, "sqlite3.dll", "x86");
//        Assembly.LoadFrom(path);
      }
    }

    private static string WriteResourceToFile(Assembly assembly, string assemblyName, string subfolder = "")
    {
      var resourceNames = assembly.GetManifestResourceNames();
      if (!resourceNames.Any())
        return string.Empty;

      var resourcePath = string.IsNullOrWhiteSpace(subfolder) ? assemblyName : string.Format("{0}.{1}", subfolder, assemblyName);
      var resourceName = resourceNames.SingleOrDefault(s => s.EndsWith(resourcePath));
      if (string.IsNullOrWhiteSpace(resourceName))
        return string.Empty;

      using (var stream = assembly.GetManifestResourceStream(resourceName))
      {
        var block = new byte[stream.Length];
        stream.Read(block, 0, block.Length);
        var path = AppDomain.CurrentDomain.BaseDirectory;
        if (!string.IsNullOrWhiteSpace(subfolder))
          path = Path.Combine(path, subfolder);
        Directory.CreateDirectory(path);
        path = Path.Combine(path, assemblyName);
        if (File.Exists(path))
        {
          var readed = File.ReadAllBytes(path);
          using (var provider = new SHA1CryptoServiceProvider())
          {
            var readedHash = provider.ComputeHash(readed);
            var resourcesHash = provider.ComputeHash(block);
            if (Enumerable.SequenceEqual(readedHash, resourcesHash))
              return path;
          }
        }
        File.WriteAllBytes(path, block);
        return path;
      }
    }
  }
}