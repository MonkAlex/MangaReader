using System;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation.Database
{
  public class From27To28 : DatabaseConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      RunSql(@"update MangaSetting 
          set DefaultCompression = 'Volume'
          where MangaName = 'Readmanga'");

      RunSql(@"update MangaSetting
          set DefaultCompression = 'Manga'
          where MangaName = 'Acomics'");

      ConfigStorage.Instance.DatabaseConfig.MangaSettings.Update();
    }

    public From27To28()
    {
      this.Version = new Version(1, 28, 5659);
    }
  }
}