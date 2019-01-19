using System;
using System.Linq;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Convertation
{
  public class HentaichanFrom38To39 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Hentaichan>().GetSettings();
        if (setting != null)
        {
          setting.MainUri = new Uri("http://henchan.me");
          setting.Login.MainUri = setting.MainUri;
          context.Save(setting);
        }
      }
    }

    public HentaichanFrom38To39()
    {
      this.Version = new Version(1, 38, 6);
      this.CanReportProcess = true;
    }
  }
}