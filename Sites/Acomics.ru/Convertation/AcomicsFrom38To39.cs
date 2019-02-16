using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace Acomics.Convertation
{
  public class AcomicsFrom38To39 : ConfigConverter
  {
    protected override Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Acomics>().GetSettings();
        if (setting != null)
        {
          setting.MainUri = new Uri("https://acomics.ru/");
          context.Save(setting);
        }
      }

      return Task.CompletedTask;
    }

    public AcomicsFrom38To39()
    {
      this.Version = new Version(1, 38, 6);
      this.CanReportProcess = true;
    }
  }
}