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

    }

    internal static void ConvertBaseTo24(ConverterProcess process)
    {
      var thisVersion = new Version(1, 24, 0, 0);

      if (thisVersion.CompareTo(Settings.DatabaseVersion) > 0)
      {
        var readmangaCompressionMode = Environment.Session.CreateSQLQuery(@"update Mangas 
          set CompressionMode = 'Volume'
          where CompressionMode is null and Type = '2c98bbf4-db46-47c4-ab0e-f207e283142d'");
        readmangaCompressionMode.UniqueResult();

        var acomicsCompressionMode = Environment.Session.CreateSQLQuery(@"update Mangas 
          set CompressionMode = 'Manga'
          where CompressionMode is null and Type = 'f090b9a2-1400-4f5e-b298-18cd35341c34'");
        acomicsCompressionMode.UniqueResult();

        Settings.DatabaseVersion = thisVersion;
      }
    }
  }
}
