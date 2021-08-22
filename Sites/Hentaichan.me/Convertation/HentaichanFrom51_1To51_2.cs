using System;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Convertation
{
  public class HentaichanFrom51_1To51_2 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Hentaichan>().GetSettings();
        var oldMainUri = new Uri("https://hen-chan.pro/");
        var mainUri = new Uri("https://hentaichan.live/");
        if (setting != null && Equals(oldMainUri, setting.MainUri))
        {
          setting.MainUri = mainUri;
          await context.Save(setting).ConfigureAwait(false);
        }
      }
    }

    public HentaichanFrom51_1To51_2()
    {
      this.Version = new Version(1, 51, 2, 13);
      this.Name = "Обновляем ссылки на hentaichan.live...";
    }
  }
}
