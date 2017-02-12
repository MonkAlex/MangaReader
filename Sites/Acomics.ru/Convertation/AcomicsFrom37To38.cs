using System;
using System.Linq;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services.Config;

namespace Acomics.Convertation
{
  public class AcomicsFrom37To38 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var setting = ConfigStorage.GetPlugin<Acomics>().GetSettings();
      if (setting != null && setting.MainUri == null)
      {
        setting.MainUri = new Uri("http://acomics.ru/");
        setting.MangaSettingUris.Add(setting.MainUri);
        setting.Login.MainUri = setting.MainUri;
        setting.Save();
      }
    }

    public AcomicsFrom37To38()
    {
      this.Version = new Version(1, 37, 3);
    }
  }
}