using System.Data.SQLite;
using System.IO;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MangaReader.Services.Config;

namespace MangaReader.Mapping
{
  public class Environment
  {
    private const string DbFile = "storage.db";

    private static ISessionFactory sessionFactory;

    public static ISession Session;

    public static ISession OpenSession()
    {
      return sessionFactory.OpenSession();
    }

    public static bool Initialized { get; set; }

    public static void Initialize()
    {
      // TODO: подумать над другим способом явно подгрузить SQLite.
      Initialized = Equals(FunctionType.Aggregate, FunctionType.Collation);

      sessionFactory = CreateSessionFactory();
      Session = sessionFactory.OpenSession();

      Initialized = true;
    }

    private static ISessionFactory CreateSessionFactory()
    {
      return Fluently
        .Configure()
        .Database(SQLiteConfiguration.Standard.UsingFile(Path.Combine(ConfigStorage.WorkFolder, DbFile)))
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Environment>())
        .ExposeConfiguration(BuildSchema)
        .BuildSessionFactory();
    }

    private static void BuildSchema(Configuration config)
    {
      config.SetInterceptor(new BaseInterceptor());
      if (File.Exists(Path.Combine(ConfigStorage.WorkFolder, DbFile)))
        new SchemaUpdate(config).Execute(false, true);
      else
        new SchemaExport(config).Create(false, true);
    }
  }
}
