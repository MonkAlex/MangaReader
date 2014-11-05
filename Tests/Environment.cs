using System.Data.SQLite;
using System.IO;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Settings = MangaReader.Services.Settings;

namespace Tests
{
  class Environment
  {
    private const string DbFile = "\\test.db";

    public static ISessionFactory SessionFactory;

    public static void Initialize()
    {
      // TODO: подумать над другим способом явно подгрузить SQLite.
      var sqlite = FunctionType.Aggregate;
      SessionFactory = CreateSessionFactory();
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
