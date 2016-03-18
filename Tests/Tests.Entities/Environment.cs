using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;

namespace Tests.Entities
{
  [TestClass]
  public class Environment
  {

    internal static ISession Session { get; set; }

    [AssemblyInitialize]
    public static void TestInitialize(TestContext context)
    {
      var process = new ReportProcess();
      MangaReader.Core.Client.Init();
      MangaReader.Core.Client.Start(process);

      Session = MangaReader.Core.NHibernate.Mapping.Session;
    }

    [AssemblyCleanup]
    public static void TestCleanup()
    {
      MangaReader.Core.Client.Close();
    }
  }
}
