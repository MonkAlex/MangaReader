using System;
using MangaReader.Core.Convertation.Primitives;

namespace MangaReader.Core.Convertation.Config
{
  public class From27To28 : ConfigConverter
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
    }

    public From27To28()
    {
      this.Version = new Version(1, 28, 5659);
    }
  }
}