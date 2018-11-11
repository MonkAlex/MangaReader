using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaReader.Core.ApplicationControl;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.API
{
  [TestFixture]
  class ApplicationControl
  {
    [Test]
    public void SendMessage_Add()
    {
      TestCommand(Messages.AddManga);
      TestCommand(Messages.Activate);
      TestCommand(Messages.Close);
    }

    private static void TestCommand(Messages message)
    {
      var uniqueId = Guid.NewGuid().ToString("D");
      Log.Add($"Server started on '{uniqueId}'.");
      var resetEvent = new AutoResetEvent(false);
      Task.Run(() => Server.Run(uniqueId));
      var lastMessage = string.Empty;

      void OnOtherAppRunning(object sender, string les)
      {
        Log.Add($"Server receive '{les}'.");
        lastMessage = les;
        resetEvent.Set();
      }

      MangaReader.Core.Client.OtherAppRunning += OnOtherAppRunning;
      Client.Run(uniqueId, message);

      Assert.IsTrue(resetEvent.WaitOne(1000));
      MangaReader.Core.Client.OtherAppRunning -= OnOtherAppRunning;
      Assert.AreEqual(message.ToString("G"), lastMessage);
    }
  }
}
