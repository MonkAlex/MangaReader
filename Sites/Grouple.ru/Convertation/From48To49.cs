using System;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Grouple.Convertation
{
  public class From48To49 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Mintmanga>().GetSettings();
        var oldMainUri = new Uri("http://mintmanga.com");
        var mainUri = new Uri("http://mintmanga.live");
        if (setting != null && Equals(oldMainUri, setting.MainUri))
        {
          setting.MainUri = mainUri;
          await context.Save(setting).ConfigureAwait(false);
        }
      }
    }

    public From48To49()
    {
      this.Version = new Version(1, 49, 0);
      this.CanReportProcess = false;
    }

  }
}
