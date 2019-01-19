using System;
using System.Linq;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Convertation
{
  public class HentaichanFrom37To38 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Hentaichan>().GetSettings();
        if (setting != null && setting.MainUri == null)
        {
          setting.MainUri = new Uri("http://hentaichan.me/");
          context.Save(setting);
        }
      }
    }

    public HentaichanFrom37To38()
    {
      this.Version = new Version(1, 37, 3);
    }
  }
}