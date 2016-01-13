using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MangaReader
{
  internal static class ResolveAssembly
  {
    internal static Assembly ResolveInternalAssembly(object sender, ResolveEventArgs args)
    {
      var thisAssembly = Assembly.GetCallingAssembly();
      var resourceNames = thisAssembly.GetManifestResourceNames();
      var assemblyFile = string.Format(".{0}.dll", args.Name.Substring(0, args.Name.IndexOf(',')));
      var resourceName = resourceNames.FirstOrDefault(s => s.EndsWith(assemblyFile));
      if (string.IsNullOrWhiteSpace(resourceName))
      {
        assemblyFile = assemblyFile.Remove(0, 1);
        resourceName = resourceNames.First(s => s.EndsWith(assemblyFile));
      }

      using (var stream = thisAssembly.GetManifestResourceStream(resourceName))
      {
        var block = new byte[stream.Length];
        stream.Read(block, 0, block.Length);
        try
        {
          return Assembly.Load(block);
        }
        catch (FileLoadException)
        {
          assemblyFile = assemblyFile.TrimStart('.');
          File.WriteAllBytes(assemblyFile, block);
          return Assembly.LoadFrom(assemblyFile);
        }
      }
    }
  }
}