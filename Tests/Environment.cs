using System.Data.SQLite;
using System.IO;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Settings = MangaReader.Services.Settings;

namespace Tests
{
  [TestClass]
  public class Environment
  {

    internal static ISession Session { get; set; }

    [AssemblyInitialize]
    public static void TestInitialize(TestContext context)
    {
      MangaReader.Mapping.Environment.Initialize();
      Settings.Load();

      Session = MangaReader.Mapping.Environment.Session;
    }

    [AssemblyCleanup]
    public static void TestCleanup()
    {

    }

  }
}
