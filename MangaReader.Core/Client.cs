using System;
using MangaReader.Services;
using MangaReader.Update;

namespace MangaReader.Core
{
  public static class Client
  {
    public static void Init()
    {
      AppDomain.CurrentDomain.UnhandledException += (o, a) => Log.Exception(a.ExceptionObject as Exception);
      AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly.ResolveInternalAssembly;
    }

    public static void Start()
    {
      Updater.Initialize();

      var isSingle = false;
      var mtx = new System.Threading.Mutex(true, "5197317b-a6f6-4a6c-a336-6fbf8642b7bc", out isSingle);
      if (!isSingle)
      {
        Log.Add("Программа уже запущена.");
        Environment.Exit(1);
      }

      Mapping.Environment.Initialize();
      Converter.Convert();
    }
  }
}