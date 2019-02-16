using System;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Grouple.Convertation
{
  public class From42To43 : ConfigConverter
  {
    protected override Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Readmanga>().GetSettings();
        if (setting != null)
        {
          setting.Login.MainUri = new Uri("https://grouple.co/");
          context.Save(setting);
        }
      }

      return Task.CompletedTask;
    }

    public From42To43()
    {
      this.Version = new Version(1, 43, 0);
      this.CanReportProcess = false;
    }

  }
}