using System;
using System.Threading.Tasks;
using MangaReader.Core.Convertation.Primitives;

namespace MangaReader.Core.Convertation.Config
{
  public class From27To28 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      await RunSql(@"update MangaSetting 
          set DefaultCompression = 'Volume'
          where MangaName = 'Readmanga'").ConfigureAwait(false);

      await RunSql(@"update MangaSetting
          set DefaultCompression = 'Manga'
          where MangaName = 'Acomics'").ConfigureAwait(false);
    }

    public From27To28()
    {
      this.Version = new Version(1, 28, 5659);
    }
  }
}