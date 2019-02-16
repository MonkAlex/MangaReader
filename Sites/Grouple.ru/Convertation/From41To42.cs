using System;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Grouple.Convertation
{
  public class From41To42 : ConfigConverter
  {
    protected override Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Readmanga>().GetSettings();
        if (setting != null)
        {
          setting.Login.MainUri = new Uri("https://grouple.ru/");
          context.Save(setting);
        }
      }

      return Task.CompletedTask;
    }

    public From41To42()
    {
      this.Version = new Version(1, 42, 0);
      this.CanReportProcess = false;
    }
  }
}
