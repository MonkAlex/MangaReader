using System;
using MangaReader.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;

namespace MangaReader.Tests
{
  [TestClass]
  public class Environment
  {

    internal static ISession Session { get; set; }

    [AssemblyInitialize]
    public static void TestInitialize(TestContext context)
    {
      var process = new ReportProcess();
      Core.Client.Init();
      Core.Client.Start(process);

      Session = Mapping.Environment.Session;
    }

    [AssemblyCleanup]
    public static void TestCleanup()
    {

    }
  }

  public class ReportProcess : IProcess
  {
    public double Percent { get; set; }
    public bool IsIndeterminate { get; set; }
    public string Status { get; set; }
    public Version Version { get; set; }
    public ConvertState State { get; set; }
    public event EventHandler<ConvertState> StateChanged;
  }
}
