using System;
using System.Threading;
using MangaReader.Services;
using MangaReader.Update;

namespace MangaReader.Core
{
  public static class Client
  {
    private static Mutex mutex;

    public static void Init()
    {
      AppDomain.CurrentDomain.UnhandledException += (o, a) => Log.Exception(a.ExceptionObject as Exception);
      AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly.ResolveInternalAssembly;
    }

    public static void Start(IProcess process)
    {
      Updater.Initialize(process);

      var isSingle = false;
      mutex = new Mutex(true, "5197317b-a6f6-4a6c-a336-6fbf8642b7bc", out isSingle);
      if (!isSingle)
      {
        Log.Add("Программа уже запущена.");
        Environment.Exit(1);
      }

      ResolveAssembly.LoadSql();
      Mapping.Environment.Initialize(process);
      Converter.Convert(process);
    }

    public static void Close()
    {
      Mapping.Environment.Close();

      if (mutex != null && !mutex.SafeWaitHandle.IsClosed)
      {
        Log.Add("Wait mutex...");
        mutex.WaitOne();
        mutex.ReleaseMutex();
        mutex.Close();
      }
    }
  }
}