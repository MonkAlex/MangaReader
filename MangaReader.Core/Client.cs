using System;
using System.Threading;
using MangaReader.Core.Convertation;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Core.Update;

namespace MangaReader.Core
{
  public static class Client
  {
    private static Mutex mutex;

    public static void Init()
    {
      AppDomain.CurrentDomain.UnhandledException += (o, a) => Log.Exception(a.ExceptionObject as System.Exception);
    }

    public static void Start(IProcess process)
    {
      Updater.Initialize(process);

      var isSingle = false;
      mutex = new Mutex(false, "5197317b-a6f6-4a6c-a336-6fbf8642b7bc", out isSingle);
      if (!isSingle)
      {
        try
        {
          Log.Exception(new ApplicationException("Программа уже запущена."));
        }
        finally
        {
          Environment.Exit(1);
        }
      }

      ConfigStorage.RefreshPlugins();
      NHibernate.Mapping.Initialize(process);
      Converter.Convert(process);
    }

    public static void Close()
    {
//      Library.ThreadAbort();
      ConfigStorage.Instance.Save();
      ConfigStorage.Close();
      NHibernate.Mapping.Close();

      if (mutex != null && !mutex.SafeWaitHandle.IsClosed)
        mutex.Close();
    }
  }
}