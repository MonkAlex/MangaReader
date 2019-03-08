using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using Octokit;

namespace MangaReader.Core.Update
{
  public class Updater
  {
    private const int RepositoryId = 17180556;

    public static readonly string RepositoryReleaseUri = "https://github.com/MonkAlex/MangaReader/releases/latest";

    private static string UpdateFilename = Path.Combine(ConfigStorage.WorkFolder, "Update", "GitHubUpdater.Launcher.exe");

    private static string UpdateConfig = Path.Combine(ConfigStorage.WorkFolder, "Update", "MangaReader.config");

    public static Version ClientVersion = AppConfig.Version;

    public static event EventHandler<string> NewVersionFound;

    /// <summary>
    /// Запуск обновления, вызываемый до инициализации программы.
    /// </summary>
    /// <remarks>Завершает обновление и удаляет временные файлы.</remarks>
    public static async Task Initialize()
    {
      Clean();
      Log.InfoFormat("Версия приложения - {0}.", ClientVersion);
      if (ConfigStorage.Instance.AppConfig.UpdateReader)
        await StartUpdate().ConfigureAwait(false);
    }

    /// <summary>
    /// Запуск обновления.
    /// </summary>
    public static async Task StartUpdate()
    {
      try
      {
        var client = new GitHubClient(new ProductHeaderValue(string.Format("MonkAlex-{0}-{1}", RepositoryId, ClientVersion)));
        var repo = await client.Repository.Get(RepositoryId).ConfigureAwait(false);
        var productName = repo.Name;
        var release = await client.Repository.Release.GetLatest(RepositoryId).ConfigureAwait(false);
        var lastVersion = release.TagName;
        if (lastVersion != ClientVersion.ToString())
        {
          Log.Info($"Для {productName} найдена версия {lastVersion}. {RepositoryReleaseUri}");
          OnNewVersionFound(lastVersion);
        }
        else
        {
          Log.Info($"Обновления не найдены");
          return;
        }
      }
      catch (System.Exception e)
      {
        Log.Exception(e, "Не удалось проверить обновления");
        Log.Info($"Не удалось проверить последнюю версию на сайте, попробуйте обновить вручную {RepositoryReleaseUri}");
      }

      if (!File.Exists(UpdateFilename))
      {
        Log.InfoFormat($"Апдейтер не найден, скачайте обновление вручную по ссылке {RepositoryReleaseUri}");
        return;
      }
     
      var args = string.Format("--fromFile \"{0}\" --version \"{1}\" --outputFolder \"{2}\"",
        UpdateConfig, ClientVersion, ConfigStorage.WorkFolder.TrimEnd('\\'));
      Log.InfoFormat("Запущен процесс обновления: Файл '{0}', с аргументами '{1}', в папке '{2}'",
        UpdateFilename, args, ConfigStorage.WorkFolder);

      Process.Start(new ProcessStartInfo {FileName = UpdateFilename, Arguments = args});
    }

    /// <summary>
    /// Чистка файлов, используемых версией 1.38 и ниже.
    /// </summary>
    private static void Clean()
    {
      try
      {
        var updateExe = Path.Combine(ConfigStorage.WorkFolder, "update.exe");
        if (File.Exists(updateExe))
        {
          File.Delete(updateExe);
          File.Delete(Path.Combine(ConfigStorage.WorkFolder, "update.it"));
        }
      }
      catch { }
    }

    private static void OnNewVersionFound(string e)
    {
      NewVersionFound?.Invoke(null, e);
    }
  }
}
