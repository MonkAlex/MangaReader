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
      Convert24To27(process);

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
