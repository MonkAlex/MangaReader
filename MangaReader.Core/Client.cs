using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using MangaReader.Core.ApplicationControl;
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

    public static event EventHandler<string> OtherAppRunning;

    public static event EventHandler ClientBeenClosed;

    public static void Init()
    {
      // Все необработанные - логгируем.
      AppDomain.CurrentDomain.UnhandledException += (o, a) => Log.Exception(a.ExceptionObject as System.Exception);

      // Все необработанные в тасках (и забытые) - пробрасываем наружу, пусть пока падает в такой ситуации, чем зависает.
      TaskScheduler.UnobservedTaskException += (o, a) => { a.SetObserved(); throw a.Exception; };
    }

    public static void Start(IProcess process)
    {
      try
      {
        Updater.Initialize();

        ConfigStorage.RefreshPlugins();
        NHibernate.Mapping.Initialize(process);

        var isSingle = false;
        var name = ConfigStorage.Instance.DatabaseConfig.UniqueId.ToString("D");
        mutex = new Mutex(false, name, out isSingle);
        if (!isSingle)
        {
          try
          {
            OnClientBeenClosed();
            process.ProgressState = ProgressState.Error;
            process.Status = "Программа уже запущена.";
            Log.Exception(new MangaReaderException("Программа уже запущена."));

            ApplicationControl.Client.Run(name, Messages.Activate);
          }
          finally
          {
            Environment.Exit(1);
          }
        }

        Task.Run(() =>
        {
          ApplicationControl.Server.Run(name);
        });

        Converter.Convert(process);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex);
      }
      finally
      {
        process.State = ConvertState.Completed;
      }
    }

    public static void Close()
    {
      ConfigStorage.Instance.Save();
      ConfigStorage.Close();
      NHibernate.Mapping.Close();

      if (mutex != null && !mutex.SafeWaitHandle.IsClosed)
        mutex.Close();

      Log.Separator("Closed");
    }

    internal static void OnOtherAppRunning(string e)
    {
      OtherAppRunning?.Invoke(null, e);
    }

    internal static void OnClientBeenClosed()
    {
      ClientBeenClosed?.Invoke(null, EventArgs.Empty);
    }
  }
}