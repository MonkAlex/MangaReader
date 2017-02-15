using System;
using System.Threading;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Exception;
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
      // Все необработанные - логгируем.
      AppDomain.CurrentDomain.UnhandledException += (o, a) => Log.Exception(a.ExceptionObject as System.Exception);

      // Все необработанные в тасках (и забытые) - пробрасываем наружу, пусть пока падает в такой ситуации, чем зависает.
      TaskScheduler.UnobservedTaskException += (o, a) => { a.SetObserved(); throw a.Exception; };
    }

    public static void Start(IProcess process)
    {
      Updater.Initialize();

      var isSingle = false;
      mutex = new Mutex(false, "5197317b-a6f6-4a6c-a336-6fbf8642b7bc", out isSingle);
      if (!isSingle)
      {
        try
        {
          Log.Exception(new MangaReaderException("Программа уже запущена."));
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