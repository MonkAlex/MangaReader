using System;
using System.Diagnostics;
using System.IO;
using MangaReader.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;

namespace Tests
{
  [TestClass]
  public class Environment
  {
    internal static ISession Session { get; set; }

    [AssemblyInitialize]
    public static void TestInitialize(TestContext context)
    {
      Deploy(@".\..\MangaReader.Core\Library\");
      Deploy(@".\..\Sites\Bin\Publish\");
      Deploy(@".\..\Sites\Bin\Release\");
      Deploy(@".\..\Sites\Bin\Debug\");
      BeforeTestClean();

      var process = new ReportProcess();
      MangaReader.Core.Services.Config.ConfigStorage.Instance.AppConfig.UpdateReader = false;

      if (!context.FullyQualifiedTestClassName.Contains("Convertation"))
      {
        MangaReader.Core.Client.Init();
        MangaReader.Core.Client.Start(process);

        Session = MangaReader.Core.NHibernate.Mapping.Session;
      }
    }

    private static readonly string TestsDirectory = SearchTestDirectory();

    public static void Deploy(string from)
    {
      var dd = AppDomain.CurrentDomain.BaseDirectory;
      var directory = new DirectoryInfo(Path.Combine(TestsDirectory, from));
      Log.AddFormat("Copy from {0} to {1}", directory, dd);
      if (directory.Exists)
      {
        InternalDeploy(directory, dd);
      }
    }

    private static void InternalDeploy(DirectoryInfo directory, string dd)
    {
      try
      {
        foreach (var info in directory.GetDirectories())
        {
          var subFolder = Path.Combine(dd, info.Name);
          Directory.CreateDirectory(subFolder);
          foreach (var file in info.GetFiles())
          {
            file.CopyTo(Path.Combine(subFolder, file.Name), true);
          }
        }
        foreach (var file in directory.GetFiles())
        {
          file.CopyTo(Path.Combine(dd, file.Name), true);
        }
      }
      catch (Exception ex)
      {
        Trace.Write(ex);
      }
    }

    private static string SearchTestDirectory()
    {
      var directory = Directory.GetCurrentDirectory();
      for (int i = 0; i < 10; i++)
      {
        if (File.Exists(Path.Combine(directory, "MangaReader.sln")))
          break;

        directory = Directory.GetParent(directory).FullName;
      }
      return Path.GetFullPath(Path.Combine(directory, "Tests"));
    }

    public static void BeforeTestClean()
    {
      var dd = AppDomain.CurrentDomain.BaseDirectory;
      File.Delete(Path.Combine(dd, "storage.db"));
      foreach (var file in Directory.GetFiles(dd, "*.dbak*", SearchOption.TopDirectoryOnly))
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
