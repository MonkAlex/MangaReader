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
    private const string DbFile = "\\test.db";

    public static ISessionFactory SessionFactory;
    public static ISession Session;

    [AssemblyInitialize]
    public static void TestInitialize(TestContext context)
    {
      Initialize();
      MangaReader.Mapping.Environment.Session = Environment.Session;
      Settings.Load();
    }

    [AssemblyCleanup]
    public static void TestCleanup()
    {
      
    }

    public static void Initialize()
    {
      // TODO: подумать над другим способом явно подгрузить SQLite.
      var sqlite = FunctionType.Aggregate;
      sqlite.Equals(FunctionType.Collation);
      SessionFactory = CreateSessionFactory();
      Session = SessionFactory.OpenSession();
    }

    private static ISessionFactory CreateSessionFactory()
    {
      return Fluently
        .Configure()
        .Database(SQLiteConfiguration.Standard.UsingFile(Settings.WorkFolder + DbFile))
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<MangaReader.Mapping.Environment>())
        .ExposeConfiguration(BuildSchema)
        .BuildSessionFactory();
    }

    private static void BuildSchema(Configuration config)
    {
      if (File.Exists(Settings.WorkFolder + DbFile))
        File.Delete(Settings.WorkFolder + DbFile);

      new SchemaExport(config).Create(false, true);
    }
  }
}
