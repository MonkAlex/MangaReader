using System;
using MangaReader.Services;

namespace MangaReader.Mapping
{
  internal class Converting
  {

    internal static void ConvertAll(ConverterProcess process)
    {
      // 1.* To 1.24
      ConvertBaseTo24(process);

      // to 1.27.5584
      Convert24To27(process);

      // to 1.28.5659
      Convert27To28(process);

      // to 1.30.5765
      Convert29To30(process);
    }

    private static void Convert29To30(ConverterProcess process)
    {
      var version = new Version(1, 30, 5765);
      if (version.CompareTo(Settings.DatabaseVersion) > 0 && process.Version.CompareTo(version) >= 0)
      {
        var setType = Environment.Session.CreateSQLQuery(@"update Login 
          set Type = 'ec4d4cde-ef54-4b67-af48-1b7909709d5c'");
        setType.UniqueResult();

/*        var hentaiLogin = Environment.Session.CreateSQLQuery(@"update Login
          set Type = '03ceff67-1472-438a-a90a-07b44f6ffdc4'
          where Id = (select Login_id from MangaSetting where MangaName = 'Hentaichan')");
        hentaiLogin.UniqueResult();*/
      }
    }

    private static void Convert27To28(ConverterProcess process)
    {
      var version = new Version(1, 28, 5659);
      if (version.CompareTo(Settings.DatabaseVersion) > 0 && process.Version.CompareTo(version) >= 0)
      {
        var readmangaHas = Environment.Session.CreateSQLQuery(@"update MangaSetting 
          set DefaultCompression = 'Volume'
          where MangaName = 'Readmanga'");
        readmangaHas.UniqueResult();

        var acomicsHas = Environment.Session.CreateSQLQuery(@"update MangaSetting
          set DefaultCompression = 'Manga'
          where MangaName = 'Acomics'");
        acomicsHas.UniqueResult();

        Settings.MangaSettings.ForEach(s => s.Update());
      }
    }

    private static void Convert24To27(ConverterProcess process)
    {
      var version = new Version(1, 27, 5584);
      if (version.CompareTo(Settings.DatabaseVersion) > 0 && process.Version.CompareTo(version) >= 0)
      {
        var readmangaHas = Environment.Session.CreateSQLQuery(@"update Mangas 
          set HasVolumes = 1, HasChapters = 1
          where HasVolumes is null and HasChapters is null and Type = '2c98bbf4-db46-47c4-ab0e-f207e283142d'");
        readmangaHas.UniqueResult();

        var acomicsHas = Environment.Session.CreateSQLQuery(@"update Mangas 
          set HasVolumes = 0, HasChapters = 0
          where HasVolumes is null and HasChapters is null and Type = 'f090b9a2-1400-4f5e-b298-18cd35341c34'");
        acomicsHas.UniqueResult();
      }
    }

    internal static void ConvertBaseTo24(ConverterProcess process)
    {
      var version = new Version(1, 24);
      if (version.CompareTo(Settings.DatabaseVersion) > 0 && process.Version.CompareTo(version) >= 0)
      {
        var readmangaCompressionMode = Environment.Session.CreateSQLQuery(@"update Mangas 
          set CompressionMode = 'Volume'
          where CompressionMode is null and Type = '2c98bbf4-db46-47c4-ab0e-f207e283142d'");
        readmangaCompressionMode.UniqueResult();

        var acomicsCompressionMode = Environment.Session.CreateSQLQuery(@"update Mangas 
          set CompressionMode = 'Manga'
          where CompressionMode is null and Type = 'f090b9a2-1400-4f5e-b298-18cd35341c34'");
        acomicsCompressionMode.UniqueResult();
      }
    }
  }
}
