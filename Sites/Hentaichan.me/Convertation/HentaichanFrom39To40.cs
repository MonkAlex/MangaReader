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
    protected override Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Mangachan.Mangachan>().GetSettings();
        if (setting != null)
        {
          setting.MainUri = new Uri("http://mangachan.me");
          context.Save(setting);
        }
      }

      return Task.CompletedTask;
    }

    public MangachanFrom39To40()
    {
      this.Version = new Version(1, 39, 1);
      this.CanReportProcess = true;
    }
  }
}
