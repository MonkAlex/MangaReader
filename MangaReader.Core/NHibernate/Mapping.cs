using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MangaReader.Core.Convertation;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Event;
using NHibernate.Mapping;
using NHibernate.Tool.hbm2ddl;

namespace MangaReader.Core.NHibernate
{
  public static class Mapping
  {
    private const string DbFile = "storage.db";

    private static ISessionFactory sessionFactory;

    public static ISession GetSession()
    {
      if (!CurrentSessionContext.HasBind(sessionFactory))
        CurrentSessionContext.Bind(sessionFactory.OpenSession());

      var session = sessionFactory.GetCurrentSession();
      if (!session.IsOpen || !session.IsConnected)
        session = sessionFactory.OpenSession();
      session.FlushMode = FlushMode.Commit;
      return session;
    }

    public static IStatelessSession GetStatelessSession()
    {
      var session = sessionFactory.OpenStatelessSession();
      return session;
    }

    public static bool Initialized { get; set; }

    public static void Initialize(IProcess process)
    {
      Initialized = false;
      Log.Add("Connect to database...");
      process.Status = "Подключение к базе данных...";
      sessionFactory = CreateSessionFactory();
      Log.Add("Connect to database completed.");

      Initialized = true;
    }

    public static void Close()
    {
      if (sessionFactory == null || sessionFactory.IsClosed)
        return;

      Log.Add("Closing database connect.");

      Initialized = false;
      sessionFactory.Close();
    }

    private static ISessionFactory CreateSessionFactory()
    {
      return Fluently
        .Configure()
        .Database(SQLiteConfiguration.Standard.UsingFile(Path.Combine(ConfigStorage.WorkFolder, DbFile)))
        .Mappings(LoadPlugins)
        .CurrentSessionContext<AsyncLocalSessionContext>()
        .ExposeConfiguration(BuildSchema)
        .BuildSessionFactory();
    }

    private static void LoadPlugins(MappingConfiguration config)
    {
      foreach (var assembly in Helper.AllowedAssemblies())
      {
        config.FluentMappings.AddFromAssembly(assembly);
      }
    }

    private static void BuildSchema(Configuration config)
    {
      foreach (var source in config.ClassMappings.Where(m => m.Discriminator != null && m is RootClass))
      {
        source.Where = string.Format("{0} in ('{1}')", 
          source.Discriminator.ColumnIterator.Single().Text,
          string.Join("', '", source.SubclassIterator.Select(i => i.DiscriminatorValue).Concat(new []{ source.DiscriminatorValue })));
      }

      config.SetInterceptor(new BaseInterceptor());
      if (File.Exists(Path.Combine(ConfigStorage.WorkFolder, DbFile)))
        new SchemaUpdate(config).Execute(false, true);
      else
      {
        Log.Add("Database file not found, create new database.");
        new SchemaExport(config).Create(false, true);
      }
    }
  }
}
