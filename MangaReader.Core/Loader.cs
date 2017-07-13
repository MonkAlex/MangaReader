using System;
using System.Diagnostics;
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


    internal static void Init()
    {
      AppDomain.CurrentDomain.AssemblyResolve -= LibraryResolve;
      AppDomain.CurrentDomain.AssemblyResolve += LibraryResolve;
    }

    private static Assembly LibraryResolve(object sender, ResolveEventArgs args)
    {
      try
      {
        var libName = args.Name;
        if (libName.Contains(','))
          libName = libName.Substring(0, libName.IndexOf(','));
        libName = libName + ".dll";
        var path = LibPath;
        var file = new DirectoryInfo(path).GetFiles().SingleOrDefault(f => f.Name == libName);
        if (file == null)
        {
          path = PluginPath;
          file = new DirectoryInfo(path).GetFiles().SingleOrDefault(f => f.Name == libName);
          if (file == null)
            return null;
        }
        return Assembly.LoadFile(file.FullName);
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
        EventLog.WriteEntry(nameof(MangaReader), $"{args.Name} \r\n {ex}", EventLogEntryType.Error);
      }
      return null;
    }

    private static void ProcessInternetZoneOnFiles(System.Exception ex, string libraryName)
    {
      EventLog.WriteEntry(nameof(MangaReader), $"Just restart app \r\n {libraryName} \r\n {ex}", EventLogEntryType.Warning);
      foreach (var s in new[] { LibPath, PluginPath })
        foreach (var fileInfo in new DirectoryInfo(s).GetFiles())
        {
          var body = File.ReadAllBytes(fileInfo.FullName);
          File.WriteAllBytes(fileInfo.FullName, body);
        }
      Environment.Exit(ex.HResult);
    }
  }
}
