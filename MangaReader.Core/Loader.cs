using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MangaReader.Core
{
  public class Loader
  {
    private readonly Environments environments;

    public Loader(Environments environments)
    {
      this.environments = environments;
    }

    internal void Init()
    {
      AppDomain.CurrentDomain.AssemblyResolve -= LibraryResolve;
      AppDomain.CurrentDomain.AssemblyResolve += LibraryResolve;
      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= LibraryResolve;
      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += LibraryResolve;
    }

    private Assembly LibraryResolve(object sender, ResolveEventArgs args)
    {
      try
      {
        var libName = args.Name;
        if (libName.Contains(','))
          libName = libName.Substring(0, libName.IndexOf(','));
        libName = libName + ".dll";

        foreach (var folder in environments.AssemblyFolders)
        {
          var directory = new DirectoryInfo(folder);
          var file = directory.Exists ? directory.GetFiles().SingleOrDefault(f => f.Name == libName) : null;
          if (file == null)
            continue;

          // LoadFile failed with empty mapping in test and can MissingMethodException when assembly load twice.
          return Assembly.LoadFrom(file.FullName);
        }
      }
      catch (FileLoadException fle)
      {
        ProcessInternetZoneOnFiles(fle, args.Name);
      }
      catch (NotSupportedException nse)
      {
        ProcessInternetZoneOnFiles(nse, args.Name);
      }
      catch (System.Exception ex)
      {
        Console.WriteLine(ex);
      }
      return null;
    }

    private void ProcessInternetZoneOnFiles(System.Exception ex, string libraryName)
    {
      Console.WriteLine($"Just restart app \r\n {libraryName} \r\n {ex}");
      foreach (var s in new[] { environments.LibPath, environments.PluginPath })
        foreach (var fileInfo in new DirectoryInfo(s).GetFiles())
        {
          var body = File.ReadAllBytes(fileInfo.FullName);
          File.WriteAllBytes(fileInfo.FullName, body);
        }
      Environment.Exit(ex.HResult);
    }
  }
}
