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
      Mapping.Environment.Initialize();
      Services.Converter.Convert();

      Session = Mapping.Environment.Session;
    }

    [AssemblyCleanup]
    public static void TestCleanup()
    {

    }
  }
}
