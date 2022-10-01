using System;
using System.Threading.Tasks;
using MangaReader.Core.Services;

namespace MangaReader.Core
{
  public class ClientInit
  {
    public event EventHandler<string> OtherAppRunning;
    public event EventHandler ClientBeenClosed;
    public event EventHandler<Version> ClientUpdated;

    public void Init()
    {
      // Все необработанные - логгируем.
      AppDomain.CurrentDomain.UnhandledException += (o, a) => Log.Exception(a.ExceptionObject as System.Exception);

      // Все необработанные в тасках (и забытые) - пробрасываем наружу, пусть пока падает в такой ситуации, чем зависает.
      TaskScheduler.UnobservedTaskException += (o, a) =>
      {
        Log.Exception(a.Exception, "UnobservedTaskException");
        a.SetObserved();
        throw a.Exception;
      };
    }

    internal void OnClientBeenClosed()
    {
      ClientBeenClosed?.Invoke(null, EventArgs.Empty);
    }

    internal void OnClientUpdated(Version e)
    {
      ClientUpdated?.Invoke(null, e);
    }

    internal void OnOtherAppRunning(string e)
    {
      OtherAppRunning?.Invoke(null, e);
    }
  }
}
