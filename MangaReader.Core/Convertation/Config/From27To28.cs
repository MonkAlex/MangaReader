using System;
using System.Threading.Tasks;
using MangaReader.Core.Convertation.Primitives;

namespace MangaReader.Core.Convertation.Config
{
  public class From27To28 : ConfigConverter
  {
    protected override Task ProtectedConvert(IProcess process)
    {
      RunSql(@"update MangaSetting 
          set DefaultCompression = 'Volume'
          where MangaName = 'Readmanga'");

      RunSql(@"update MangaSetting
          set DefaultCompression = 'Manga'
          where MangaName = 'Acomics'");

      return Task.CompletedTask;
    }

    public From27To28()
    {
      this.Version = new Version(1, 28, 5659);
    }
  }
}