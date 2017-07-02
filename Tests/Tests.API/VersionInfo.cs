using System;
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
}
