using System;
using System.Threading;
using System.Threading.Tasks;
using MangaReader.Core.ApplicationControl;
using MangaReader.Core.Convertation;
using MangaReader.Core.Exception;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Core.Update;

namespace MangaReader.Core
{
  public class Client
  {
    private Mutex mutex;

    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private readonly JsonConfigStorage storage;
    private readonly PluginManager pluginManager;
    private readonly Mapping mapping;
    private readonly ClientInit clientInit;
    private readonly Updater updater;

    public Client(JsonConfigStorage storage, PluginManager pluginManager, Mapping mapping, ClientInit clientInit, Updater updater)
    {
      this.storage = storage;
      this.pluginManager = pluginManager;
      this.mapping = mapping;
      this.clientInit = clientInit;
      this.updater = updater;
    }

    public async Task Start(IProcess process)
    {
      try
      {
        await updater.Initialize().ConfigureAwait(false);

        pluginManager.RefreshPlugins();
        mapping.Initialize(process);
        await DatabaseConfig.Initialize().ConfigureAwait(false);

        var name = NHibernate.Repository.GetDatabaseUniqueId().ToString("D");
        mutex = new Mutex(false, $"Global\\{name}", out bool isSingle);
        if (!isSingle)
        {
          try
          {
            clientInit.OnClientBeenClosed();
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

        // Сервер стартует отдельно и рекурсивно сам себя поддерживает. Ждать его не нужно и нет никакого смысла.
#pragma warning disable 4014
        ApplicationControl.Server.RunTask(clientInit, name, cancellationTokenSource.Token);
#pragma warning restore 4014

        await Converter.Convert(process).ConfigureAwait(false);
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

    public void Close()
    {
      storage.Save();
      mapping.Close();

      if (mutex != null && !mutex.SafeWaitHandle.IsClosed)
        mutex.Close();

      if (!cancellationTokenSource.IsCancellationRequested)
        cancellationTokenSource.Cancel();

      Log.Separator("Closed");
    }
  }
}
