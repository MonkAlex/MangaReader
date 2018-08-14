using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Convertation
{
  public class MangachanFrom39To40 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Mangachan.Mangachan>().GetSettings();
        if (setting != null)
        {
          setting.MainUri = new Uri("http://mangachan.me");
          setting.MangaSettingUris.Add(setting.MainUri);
          setting.Login.MainUri = setting.MainUri;
          context.Save(setting);
        }
      }
    }

    public MangachanFrom39To40()
    {
      this.Version = new Version(1, 39, 1);
      this.CanReportProcess = true;
    }
  }
}
