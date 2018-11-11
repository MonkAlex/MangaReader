using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MangaReader.Core
{
  internal class Loader
  {
    /// <summary>
    /// Папка программы.
    /// </summary>
    internal static string WorkFolder
    {
      get { return workFolder; }
    }

    private static string workFolder = AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// Папка с либами программы.
    /// </summary>
    internal static string LibPath
    {
      get { return Path.Combine(WorkFolder, "lib"); }
    }

    /// <summary>
    /// Папка с плагинами программы.
    /// </summary>
    internal static string PluginPath
    {
      get { return Path.Combine(WorkFolder, "Plugins"); }
    }

    internal static string[] AssemblyFolders
    {
      get { return new string[] {LibPath, PluginPath}; }
    }


    internal static void Init()
    {
      AppDomain.CurrentDomain.AssemblyResolve -= LibraryResolve;
      AppDomain.CurrentDomain.AssemblyResolve += LibraryResolve;
      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= LibraryResolve;
      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += LibraryResolve;
    }

    private static Assembly LibraryResolve(object sender, ResolveEventArgs args)
    {
      try
      {
        var libName = args.Name;
        if (libName.Contains(','))
          libName = libName.Substring(0, libName.IndexOf(','));
        libName = libName + ".dll";

        foreach (var folder in AssemblyFolders)
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

    private static void ProcessInternetZoneOnFiles(System.Exception ex, string libraryName)
    {
      Console.WriteLine($"Just restart app \r\n {libraryName} \r\n {ex}");
      foreach (var s in new[] {LibPath, PluginPath})
      foreach (var fileInfo in new DirectoryInfo(s).GetFiles())
      {
        var body = File.ReadAllBytes(fileInfo.FullName);
        File.WriteAllBytes(fileInfo.FullName, body);
      }
      Environment.Exit(ex.HResult);
    }
  }
}