using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Convertation
{
  [TestClass]
  public static class Environment
  {
    private static string deploymentDirectory;

    private static string testsDirectory;
    
    public static void Deploy(string from)
    {
      var directory = new DirectoryInfo(Path.Combine(testsDirectory, from));
      if (directory.Exists)
      {
        foreach (var file in directory.GetFiles())
        {
          file.CopyTo(Path.Combine(deploymentDirectory, file.Name), true);
        }
      }
    }

    [AssemblyInitialize]
    public static void TestInitialize(TestContext context)
    {
      deploymentDirectory = context.DeploymentDirectory;
      testsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Tests");
      TestCleanup();
      BeforeTestClean();
    }

    public static void BeforeTestClean()
    {
      File.Delete(Path.Combine(deploymentDirectory, "storage.db"));
      foreach (var file in Directory.GetFiles(deploymentDirectory, "*.dbak*", SearchOption.TopDirectoryOnly))
      {
        File.Delete(file);
      }
    }

    [AssemblyCleanup]
    public static void TestCleanup()
    {
      MangaReader.Core.Client.Close();
    }
  }
}
