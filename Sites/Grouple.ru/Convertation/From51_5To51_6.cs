using System;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Grouple.Convertation
{
  public class From51_5To51_6 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Readmanga>().GetSettings();
        var oldMainUri = new Uri("https://readmanga.io/");
        var mainUri = new Uri("https://readmanga.live/");
        if (setting != null && Equals(oldMainUri, setting.MainUri))
        {
          setting.MainUri = mainUri;
          await context.Save(setting).ConfigureAwait(false);
        }
      }
    }

    public From51_5To51_6()
    {
      this.Version = new Version(1, 51, 6, 23);
      this.CanReportProcess = false;
      this.Name = "Обновляем ссылки на readmanga.live...";
    }

  }
}
