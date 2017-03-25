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
      catch (System.Exception ex)
      {
        EventLog.WriteEntry(nameof(MangaReader), $"{args.Name} \r\n {ex}", EventLogEntryType.Error);
      }
      return null;
    }
  }
}
