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
    private const string DbFile = "\\storage.db";

    private static ISessionFactory SessionFactory;

    public static ISession Session;

    public static ISession OpenSession()
    {
      return SessionFactory.OpenSession();
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
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Environment>())
        .ExposeConfiguration(BuildSchema)
        .BuildSessionFactory();
    }

    private static void BuildSchema(Configuration config)
    {
      config.SetInterceptor(new BaseInterceptor());
      if (File.Exists(Settings.WorkFolder + DbFile))
        new SchemaUpdate(config).Execute(false, true);
      else
        new SchemaExport(config).Create(false, true);
    }
  }
}
