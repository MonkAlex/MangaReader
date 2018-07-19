using System;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Grouple.Convertation
{
  public class From42To43 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      using (Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Readmanga>().GetSettings();
        if (setting != null)
        {
          setting.Login.MainUri = new Uri("https://grouple.co/");
          setting.Save();
        }
      }
    }

    public From42To43()
    {
      this.Version = new Version(1, 43, 0);
      this.CanReportProcess = false;
    }

  }
}