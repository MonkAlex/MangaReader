using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Convertation
{
  public class HenchanFrom43To44 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Hentaichan>().GetSettings();
        if (setting != null)
        {
          setting.MainUri = new Uri("http://hentai-chan.me");
          await context.Save(setting).ConfigureAwait(false);
        }
      }
    }

    public HenchanFrom43To44()
    {
      this.Version = new Version(1, 43, 5);
      this.CanReportProcess = true;
    }
  }
}