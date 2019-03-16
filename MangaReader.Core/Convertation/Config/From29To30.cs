using System;
using System.Threading.Tasks;
using MangaReader.Core.Convertation.Primitives;

namespace MangaReader.Core.Convertation.Config
{
  public class From29To30 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      await RunSql(@"update Login 
          set Type = 'f526cd85-7846-4f32-85a7-c57e3983dfb1'
          where Id = (select Login_id from MangaSetting where MangaName = 'Acomics')").ConfigureAwait(false);

      await RunSql(@"update Login
          set Type = '03ceff67-1472-438a-a90a-07b44f6ffdc4'
          where Id = (select Login_id from MangaSetting where MangaName = 'Hentaichan')").ConfigureAwait(false);

      await RunSql(@"update Login
          set Type = '0bbe71b1-16e0-44f4-b7c6-3450e44e9a15'
          where Id = (select Login_id from MangaSetting where MangaName = 'Readmanga')").ConfigureAwait(false);
    }

    public From29To30()
    {
      this.Version = new Version(1, 30, 5765);
    }
  }
}