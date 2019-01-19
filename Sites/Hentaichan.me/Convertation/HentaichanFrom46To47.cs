using System;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Convertation
{
  public class HentaichanFrom46To47 : ConfigConverter
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
          context.Save(setting);
        }
      }
    }

    public HentaichanFrom46To47()
    {
      this.Version = new Version(1, 47, 3);
      this.CanReportProcess = true;
      this.Name = "Обновляем ссылки на henchan.me...";
    }
  }
}