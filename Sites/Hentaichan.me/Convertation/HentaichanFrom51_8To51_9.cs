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
  internal class HentaichanFrom51_8To51_9 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Hentaichan>().GetSettings();
        var mainUri = new Uri("https://x.henchan.pro/");
        if (setting != null)
        {
          setting.MainUri = mainUri;
          await context.Save(setting).ConfigureAwait(false);
        }
      }
    }

    public HentaichanFrom51_8To51_9()
    {
      this.Version = new Version(1, 51, 9, 39);
      this.CanReportProcess = false;
      this.Name = "Обновляем ссылки на x.henchan.pro...";
    }
  }
}
