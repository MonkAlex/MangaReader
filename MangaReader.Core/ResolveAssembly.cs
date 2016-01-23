using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MangaReader.Core
{
  internal static class ResolveAssembly
  {
    internal static Assembly ResolveInternalAssembly(object sender, ResolveEventArgs args)
    {
      var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("MangaReader")).ToList();
      foreach (var assembly in assemblies)
      {
        var resourceNames = assembly.GetManifestResourceNames();
        if (!resourceNames.Any())
          continue;

        var assemblyFile = string.Format(".{0}.dll", args.Name.Substring(0, args.Name.IndexOf(',')));
        var resourceName = resourceNames.SingleOrDefault(s => s.EndsWith(assemblyFile));
        if (string.IsNullOrWhiteSpace(resourceName))
        {
          assemblyFile = assemblyFile.Remove(0, 1);
          resourceName = resourceNames.SingleOrDefault(s => s.EndsWith(assemblyFile));
        }

        if (string.IsNullOrWhiteSpace(resourceName))
          continue;

        return Load(assembly, resourceName, assemblyFile);
      }
      return null;
    }

    private static Assembly Load(Assembly from, string resourceName, string assemblyName)
    {
      using (var stream = from.GetManifestResourceStream(resourceName))
      {
        var block = new byte[stream.Length];
        stream.Read(block, 0, block.Length);
        try
        {
          return Assembly.Load(block);
        }
        catch (FileLoadException)
        {
          assemblyName = assemblyName.TrimStart('.');
          var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), assemblyName);
          File.WriteAllBytes(path, block);
          return Assembly.LoadFrom(path);
        }
      }
    }
  }
}