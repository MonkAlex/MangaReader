using System;
using System.Linq;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Convertation
{
  public class HentaichanFrom37To38 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var setting = ConfigStorage.Instance.DatabaseConfig.MangaSettings.SingleOrDefault(s => s.MainUri == null && s.Manga == Hentaichan.Type);
      if (setting != null)
      {
        setting.MainUri = new Uri("http://hentaichan.me/");
        setting.MangaSettingUris.Add(setting.MainUri);
        setting.MangaSettingUris.Add(new Uri("http://hentaichan.ru/"));
        setting.Login.MainUri = setting.MainUri;
        setting.Save();
      }
    }

    public HentaichanFrom37To38()
    {
      this.Version = new Version(1, 37, 3);
    }
  }
}