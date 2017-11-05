using System;
using System.Linq;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Convertation
{
  public class HenchanFrom43To44 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var setting = ConfigStorage.GetPlugin<Hentaichan>().GetSettings();
      if (setting != null)
      {
        setting.MainUri = new Uri("http://hentai-chan.me");
        setting.MangaSettingUris.Add(setting.MainUri);
        setting.Login.MainUri = setting.MainUri;
        setting.Save();
      }

      var mangas = Repository.Get<Hentaichan>().ToList();
      foreach (var manga in mangas)
      {
        manga.Uri = new Uri(manga.Uri.OriginalString.Replace("henchan.me", "hentai-chan.me"));
        process.Percent += 100.0 / mangas.Count;
      }
      mangas.SaveAll();
    }

    public HenchanFrom43To44()
    {
      this.Name = "henchan.me переехал на hentai-chan.me...";
      this.Version = new Version(1, 43, 5);
      this.CanReportProcess = true;
    }
  }
}