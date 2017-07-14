using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests
{
  public class Environment
  {
    public static Lazy<bool> SetUpLazy = ValueFactory();

    private static Lazy<bool> ValueFactory()
    {
      return new Lazy<bool>(() =>
      {
        SetUpInternal(InitClient);
        return true;
      });
    }

    public static bool InitClient = false;
    
    public static void SetUp(bool initSession)
    {
      InitClient = initSession;
      if (SetUpLazy.Value)
      {
        TestContext.Progress.Write("Already Set Up.");
      }
    }

    private static void SetUpInternal(bool initSession)
    {
      BeforeTestClean();
      DeployToLib(@".\..\MangaReader.Core\Library\");
      DeployToLib(@".\..\Sites\Bin\Publish\");
      DeployToLib(@".\..\Sites\Bin\Release\");
      DeployToLib(@".\..\Sites\Bin\Debug\");

      MangaReader.Core.Loader.Init();
      MangaReader.Core.Services.Config.ConfigStorage.Instance.AppConfig.UpdateReader = false;

      if (initSession && !Mapping.Initialized)
      {
        var process = new ReportProcess();
        MangaReader.Core.Client.Init();
        MangaReader.Core.Client.Start(process);
      }
    }

    private static readonly string TestsDirectory = SearchTestDirectory();

    public static void DeployToLib(string from)
    {
      DeployTo(from, "Lib");
    }

    public static void DeployToRoot(string from)
    {
      DeployTo(from, string.Empty);
    }

    private static void DeployTo(string from, string to)
    {
      var dd = Path.Combine(TestContext.CurrentContext.TestDirectory, to);
      var directory = new DirectoryInfo(Path.GetFullPath(Path.Combine(TestsDirectory, from)));
      Log.AddFormat("Copy from {0} to {1}", directory, dd);
      if (directory.Exists)
      {
        InternalDeploy(directory, dd);
      }
      Directory.SetCurrentDirectory(dd);
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
            try
            {
              var dest = Path.Combine(subFolder, file.Name);
              if (!File.Exists(dest))
                file.CopyTo(dest, false);
            }
            catch (Exception ex)
            {
              TestContext.Progress.Write(ex);
            }
          }
        }
        foreach (var file in directory.GetFiles())
        {
          try
          {
            var dest = Path.Combine(dd, file.Name);
            if (!File.Exists(dest))
              file.CopyTo(dest, false);
          }
          catch (Exception ex)
          {
            TestContext.Progress.Write(ex);
          }
        }
      }
      catch (Exception ex)
      {
        TestContext.Progress.Write(ex);
      }
    }

    private static string SearchTestDirectory()
    {
      var directory = AppDomain.CurrentDomain.BaseDirectory;
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
      var masks = new[] {"System.Data.SQLite*", "*hibernate*", "*.dbak"};
      foreach (var mask in masks)
      {
        foreach (var file in Directory.GetFiles(dd, mask, SearchOption.TopDirectoryOnly))
        {
          File.Delete(file);
        }        
      }
    }

    public static void TestCleanup()
    {
      MangaReader.Core.Client.Close();
      SetUpLazy = ValueFactory();
    }
  }

  [Parallelizable(ParallelScope.All)]
  public class TestClass
  {
    [SetUp]
    protected void SetUp()
    {
      Environment.SetUp(true);
    }

    [TearDown]
    protected void Clean()
    {
    }
  }
}