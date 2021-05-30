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
      var token = new CancellationTokenSource();
      TestCommand(Messages.AddManga, token.Token);
      TestCommand(Messages.Activate, token.Token);
      TestCommand(Messages.Close, token.Token);
      token.Cancel();
    }

    private static void TestCommand(Messages message, CancellationToken token)
    {
      var uniqueId = Guid.NewGuid().ToString("D");
      Log.Add($"Server started on '{uniqueId}'.");
      var resetEvent = new AutoResetEvent(false);
      Server.RunTask(uniqueId, token);
      var lastMessage = string.Empty;

      void OnOtherAppRunning(object sender, string les)
      {
        Log.Add($"Server receive '{les}'.");
        lastMessage = les;
        resetEvent.Set();
      }

      MangaReader.Core.Client.OtherAppRunning += OnOtherAppRunning;
      Client.Run(uniqueId, message);

      Assert.IsTrue(resetEvent.WaitOne(100));
      MangaReader.Core.Client.OtherAppRunning -= OnOtherAppRunning;
      Assert.AreEqual(message.ToString("G"), lastMessage);
    }
  }
}
