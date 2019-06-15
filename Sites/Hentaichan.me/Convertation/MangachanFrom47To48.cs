using System;
using System.Threading.Tasks;
using System.Linq;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Convertation
{
  public class MangachanFrom47To48 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Mangachan.Mangachan>().GetSettings();
        var oldMainUri = new Uri("http://mangachan.me/");
        var mainUri = new Uri("https://manga-chan.me/");
        if (setting != null && Equals(oldMainUri, setting.MainUri))
        {
          setting.MainUri = mainUri;

          var mangas = context.Get<Mangachan.Mangachan>().ToList();
          foreach (var manga in mangas)
          {
            var parsed = manga.Parser.ParseUri(manga.Uri);
            if (parsed.CanBeParsed)
              continue;

            var splitted = manga.Uri.OriginalString.Split('/');
            if (splitted.Length > 3 && string.IsNullOrEmpty(splitted[3]))
            {
              splitted[3] = "manga";
              manga.Uri = new Uri(string.Join("/", splitted));
            }
          }
          await context.Save(setting).ConfigureAwait(false);
        }
      }
    }

    public MangachanFrom47To48()
    {
      this.Version = new Version(1, 48, 0);
      this.Name = "Обновляем ссылки на mangachan.me...";

    }
  }
}
