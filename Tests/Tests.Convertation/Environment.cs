using System.IO;
using System.Reflection;
using MangaReader.Core.Services;
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
      Log.AddFormat("Copy from {0} to {1}", directory, deploymentDirectory);
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
      // TODO: Just select test setting file, if it NRE.
      var test = context.GetType().GetField("m_test", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(context);
      var path = (string)test.GetType().GetProperty("CodeBase", BindingFlags.Public | BindingFlags.Instance).GetValue(test);
      deploymentDirectory = context.DeploymentDirectory;
      testsDirectory = Path.GetFullPath(Path.Combine(path, "..", "..", "..", ".."));
      BeforeTestClean();
      MangaReader.Core.Services.Config.ConfigStorage.Instance.AppConfig.UpdateReader = false;
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
