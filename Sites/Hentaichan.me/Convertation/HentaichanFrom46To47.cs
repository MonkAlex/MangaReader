using System;
using System.Linq;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
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
          setting.Login.MainUri = setting.MainUri;
          context.Save(setting);
        }

        var mangas = context.Get<Hentaichan>().ToList();
        foreach (var manga in mangas)
        {
          manga.Uri = new Uri(manga.Uri.OriginalString.Replace("hentai-chan.me", "henchan.me"));
          process.Percent += 100.0 / mangas.Count;
        }
        mangas.SaveAll(context);
      }
    }

    public HentaichanFrom46To47()
    {
      this.Version = new Version(1, 47, 3);
      this.CanReportProcess = true;
    }
  }
}