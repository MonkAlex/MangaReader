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

    public static ISession Session;

    public static void Initialize()
    {
      // TODO: подумать над другим способом явно подгрузить SQLite.
      var sqlite = FunctionType.Aggregate;
      SessionFactory = CreateSessionFactory();
      Session = SessionFactory.OpenSession();
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
      if (File.Exists(Settings.WorkFolder + DbFile))
        return;
      //  File.Delete(Settings.WorkFolder + DbFile);

      new SchemaExport(config).Create(false, true);
    }
  }
}
