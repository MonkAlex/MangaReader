using System.IO;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MangaReader.Services;
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

    public static void Initialize(IProcess process)
    {
      Initialized = false;

      process.Status = "Подключение к базе данных...";
      sessionFactory = CreateSessionFactory();
      Session = sessionFactory.OpenSession();

      Initialized = true;
    }

    public static void Close()
    {
      if (sessionFactory == null || sessionFactory.IsClosed)
        return;

      sessionFactory.Close();
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
