using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Tests.API
{
  [TestFixture]
  public class VersionInfo
  {
    [Test]
    public void VersionHistoryIsExists()
    {
      var text = MangaReader.Core.Update.VersionHistory.GetHistory();
      Assert.IsTrue(!string.IsNullOrWhiteSpace(text));
    }

    [Test]
    public void VersionHistoryCorrect()
    {
      var version = MangaReader.Core.Update.VersionHistory.GetVersion();
      Assert.AreEqual(MangaReader.Core.Services.Config.AppConfig.Version, version);
    }
  }

  [TestFixture]
  public class Updater
  {
    [Test]
    public async Task ChechUpdates()
    {
      var old = MangaReader.Core.Update.Updater.ClientVersion;
      try
      {
        MangaReader.Core.Update.Updater.ClientVersion = new Version(1, 0, 0, 0);
        var found = false;
        void UpdaterOnNewVersionFound(object sender, string e)
        {
          found = true;
        }

        MangaReader.Core.Update.Updater.NewVersionFound += UpdaterOnNewVersionFound;
        await MangaReader.Core.Update.Updater.StartUpdate().ConfigureAwait(false);
        Assert.IsTrue(found);
      }
      catch (Exception e)
      {
        MangaReader.Core.Update.Updater.ClientVersion = old;
      }
    }

  }
}
