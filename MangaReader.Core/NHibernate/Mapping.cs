using System.IO;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MangaReader.Services;
using MangaReader.Services.Config;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace MangaReader.Core.NHibernate
{
  public class Mapping
  {
    private const string DbFile = "storage.db";

    private static ISessionFactory sessionFactory;

    public static ISession Session;

    public static ISession OpenSession()
    {
      var session = sessionFactory.OpenSession();
      session.FlushMode = FlushMode.Commit;
      return session;
    }

    public static bool Initialized { get; set; }

    public static void Initialize(IProcess process)
    {
      Initialized = false;

      process.Status = "Подключение к базе данных...";
      sessionFactory = CreateSessionFactory();
      Session = OpenSession();

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
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Mapping>())
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
