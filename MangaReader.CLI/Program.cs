using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MangaReader.CLI
{
  class Program
  {
    static void Main(string[] args)
    {
      AppDomain.CurrentDomain.AssemblyResolve += LibSubfolderResolve;
      Initialize.Run();
    }

    private static Assembly LibSubfolderResolve(object sender, ResolveEventArgs args)
    {
      var libName = args.Name;
      if (libName.Contains(','))
        libName = libName.Substring(0, libName.IndexOf(','));
      libName = libName + ".dll";
      var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib");
      var file = new DirectoryInfo(path).GetFiles().SingleOrDefault(f => f.Name == libName);
      if (file == null)
        return null;
      return Assembly.LoadFile(file.FullName);
    }
  }
}
