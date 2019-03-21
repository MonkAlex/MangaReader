using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.NHibernate;

namespace MangaReader.Core.Convertation.Mangas
{
  public class From37To38 : MangasConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && this.CanConvertVersion(process);
    }

    protected override async Task ProtectedConvert(IProcess process)
    {
      await RunSql(@"update Mangas
               set Type = '64ac91ef-bdb3-4086-be17-bb1dbe7a7656'
               where Uri like '%mintmanga.com%' or Uri like '%adultmanga.ru%'").ConfigureAwait(false);

      await RunSql(@"update Mangas
               set Setting = (select Id from MangaSetting where MangaName = 'Mintmanga')
               where Type = '64ac91ef-bdb3-4086-be17-bb1dbe7a7656'").ConfigureAwait(false);

      var mainHosts = Repository.GetStateless<Services.MangaSetting>().Select(s => s.MainUri.Host).Distinct().ToList();
      using (var context = Repository.GetEntityContext())
      {
        foreach (var manga in await context.Get<IManga>().ToListAsync().ConfigureAwait(false))
        {
          if (mainHosts.Contains(manga.Uri.Host))
            continue;

          var newHost = manga.Setting.MainUri.Host;
          var builder = new UriBuilder(manga.Uri) { Host = newHost, Port = -1 };
          manga.Uri = builder.Uri;
          await context.Save(manga).ConfigureAwait(false);
        }
      }
    }

    public From37To38()
    {
      this.Version = new Version(1, 37, 3);
    }
  }
}