using System;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Grouple.Convertation
{
  public class From49_8To49_9 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Readmanga>().GetSettings();
        var oldMainUri = new Uri("https://readmanga.me/");
        var mainUri = new Uri("https://readmanga.live/");
        if (setting != null && Equals(oldMainUri, setting.MainUri))
        {
          setting.MainUri = mainUri;
          await context.Save(setting).ConfigureAwait(false);
        }
      }
    }

    public From49_8To49_9()
    {
      this.Version = new Version(1, 49, 9, 62);
      this.CanReportProcess = false;
      this.Name = "Обновляем ссылки на readmanga.live...";
    }

  }
}
