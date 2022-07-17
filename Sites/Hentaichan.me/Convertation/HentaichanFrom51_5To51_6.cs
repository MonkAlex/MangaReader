using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Hentaichan.me.Convertation
{
  public class HentaichanFrom51_5To51_6 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Hentaichan>().GetSettings();
        var oldMainUris = new[] { 
          new Uri("https://hentaichan.live/"),
          new Uri("https://x.hentaichan.live/"),
          new Uri("https://xx.hentaichan.live/"),
          new Uri("https://xxx.hentaichan.live/"),
          new Uri("https://xxxx.hentaichan.live/"),
        };
        var mainUri = new Uri("https://y.hentaichan.live/");
        if (setting != null && oldMainUris.Any(oldMainUri => Equals(oldMainUri, setting.MainUri)))
        {
          setting.MainUri = mainUri;
          await context.Save(setting).ConfigureAwait(false);
        }
      }
    }

    public HentaichanFrom51_5To51_6()
    {
      this.Version = new Version(1, 51, 6, 23);
      this.CanReportProcess = false;
      this.Name = "Обновляем ссылки на hentaichan.live...";
    }
  }
}
