using System;
using MangaReader.Account;
using MangaReader.Services;

namespace MangaReader.Update
{
  public class UpdaterWPF
  {
    static UpdaterWPF()
    {
      Updater.UpdateDownloadStarted += UpdaterOnUpdateDownloadStarted;
      Updater.UpdateCompleted += UpdaterOnUpdateCompleted;
    }

    private static void UpdaterOnUpdateDownloadStarted(object sender, CookieClient e)
    {
      var download = new Download(WindowHelper.Owner);
      e.DownloadProgressChanged += (s, args) => download.UpdateStates(args);
      e.DownloadDataCompleted += (s, args) => download.Close();
      download.ShowDialog();
    }

    private static void UpdaterOnUpdateCompleted(object sender, EventArgs eventArgs)
    {
      new VersionHistory().ShowDialog();
    }

    public static void Initialize()
    {
      Updater.Initialize();
    }
  }
}