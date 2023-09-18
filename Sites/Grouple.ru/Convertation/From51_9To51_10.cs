using System;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Grouple.ru.Convertation
{
  public class From51_9To51_10 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Mintmanga>().GetSettings();
        var oldMainUri = new Uri("https://mintmanga.live/");
        var mainUri = new Uri("https://m.mintmanga.live/");
        if (setting != null && Equals(oldMainUri, setting.MainUri))
        {
          setting.MainUri = mainUri;
          await context.Save(setting).ConfigureAwait(false);
        }
      }
    }

    public From51_9To51_10()
    {
      this.Version = new Version(1, 51, 10, 39);
      this.CanReportProcess = false;
      this.Name = "Обновляем ссылки на m.mintmanga.live...";
    }
  }
}
