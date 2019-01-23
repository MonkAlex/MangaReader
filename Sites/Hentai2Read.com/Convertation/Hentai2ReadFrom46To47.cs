using System;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Hentai2Read.com.Convertation
{
  public class Hentai2ReadFrom46To47 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Hentai2Read>().GetSettings();
        if (setting != null)
        {
          setting.MainUri = new Uri("https://hentai2read.com");
          context.Save(setting);
        }
      }
    }

    public Hentai2ReadFrom46To47()
    {
      this.Version = new Version(1, 47, 4);
      this.Name = "Обновляем ссылки на hentai2read.com";
    }
  }
}