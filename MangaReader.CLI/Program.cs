using System;

namespace MangaReader.CLI
{
  class Program
  {
    static void Main(string[] args)
    {
      AppDomain.CurrentDomain.AssemblyResolve += Core.ResolveAssembly.ResolveInternalAssembly;
      Initialize.Run();
    }
  }
}
