using System.Data.SQLite;
using System.IO;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Settings = MangaReader.Services.Settings;

namespace MangaReader.Mapping
{
  public class Environment
  {
    private const string DbFile = "\\firstProject.db";

    public static ISessionFactory SessionFactory;

    public static void Initialize()
    {
      // TODO: подумать над другим способом явно подгрузить SQLite.
      var sqlite = FunctionType.Aggregate;
      SessionFactory = CreateSessionFactory();
    }

    public static ISession OpenSession()
    {
      return SessionFactory.OpenSession();
    }

    private static ISessionFactory CreateSessionFactory()
    {
      return Fluently
        .Configure()
        .Database(SQLiteConfiguration.Standard.UsingFile(Settings.WorkFolder + DbFile))
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Environment>())
        .ExposeConfiguration(BuildSchema)
        .BuildSessionFactory();
    }

    private static void BuildSchema(Configuration config)
    {
      // delete the existing db on each run
      if (File.Exists(Settings.WorkFolder + DbFile))
        return;
      //  File.Delete(Settings.WorkFolder + DbFile);

      // this NHibernate tool takes a configuration (with mapping info in)
      // and exports a database schema from it
      new SchemaExport(config).Create(false, true);
    }
  }
}
