using System;
using System.Threading.Tasks;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services;

namespace MangaReader.Core.Convertation.Database
{
  public class FromBaseTo24 : DatabaseConverter
  {
    protected override Task ProtectedConvert(IProcess process)
    {
      RunSql(@"update Mangas 
          set CompressionMode = 'Volume'
          where CompressionMode is null and Type = '2c98bbf4-db46-47c4-ab0e-f207e283142d'");

      RunSql(@"update Mangas 
          set CompressionMode = 'Manga'
          where CompressionMode is null and Type = 'f090b9a2-1400-4f5e-b298-18cd35341c34'");

      return Task.CompletedTask;
    }

    public FromBaseTo24()
    {
      this.Version = new Version(1, 24);
    }
  }
}