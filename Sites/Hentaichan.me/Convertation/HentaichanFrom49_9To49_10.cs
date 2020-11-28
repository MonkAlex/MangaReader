using System;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Convertation
{
  public class HentaichanFrom49_9To49_10 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Hentaichan>().GetSettings();
        var oldMainUri = new Uri("https://henchan.pro/");
        var mainUri = new Uri("https://hentai-chan.pro/");
        if (setting != null && Equals(oldMainUri, setting.MainUri))
        {
          setting.MainUri = mainUri;
          await context.Save(setting).ConfigureAwait(false);
        }
      }
    }

    public HentaichanFrom49_9To49_10()
    {
      this.Version = new Version(1, 49, 10, 63);
      this.Name = "Обновляем ссылки на hentai-chan.pro...";
    }
  }
}
