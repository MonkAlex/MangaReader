using System;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services.Config;

namespace Grouple.Convertation
{
  public class From41To42 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var setting = ConfigStorage.GetPlugin<Readmanga>().GetSettings();
      if (setting != null)
      {
        setting.Login.MainUri = new Uri("https://grouple.ru/");
        setting.Save();
      }
    }

    public From41To42()
    {
      this.Version = new Version(1, 42, 0);
      this.CanReportProcess = false;
    }
  }
}
