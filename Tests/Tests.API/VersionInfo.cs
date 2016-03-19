using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.API
{
  [TestClass]
  public class VersionInfo
  {
    [TestMethod]
    public void VersionHistoryIsExists()
    {
      var text = MangaReader.Core.Update.VersionHistory.GetHistory();
      Assert.IsTrue(!string.IsNullOrWhiteSpace(text));
    }

    [TestMethod]
    public void VersionHistoryCorrect()
    {
      var version = MangaReader.Core.Update.VersionHistory.GetVersion();
      Assert.AreEqual(MangaReader.Core.Services.Config.AppConfig.Version, version);
    }
  }
}
