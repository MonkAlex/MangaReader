using System;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using System.Linq;
using System.Threading.Tasks;

namespace MangaReader.Core.Convertation.History
{
  public class From32To33 : HistoryConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && this.CanConvertVersion(process);
    }

    protected override async Task ProtectedConvert(IProcess process)
    {
      await RunSql(@"update MangaHistory
               set Uri = Replace(Uri, ':80/', '/')
               where Uri like '%:80/%'").ConfigureAwait(false);
    }

    public From32To33()
    {
      this.Version = new Version(1, 33, 5789);
    }
  }
}