using System;
using System.Linq;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;

namespace MangaReader.Core.Convertation.Mangas
{
  public class From37To38 : MangasConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && this.CanConvertVersion(process);
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      RunSql(@"update Mangas
               set Type = '64ac91ef-bdb3-4086-be17-bb1dbe7a7656'
               where Uri like '%mintmanga.com%' or Uri like '%adultmanga.ru%'");

      RunSql(@"update Mangas
               set Setting = (select Id from MangaSetting where MangaName = 'Mintmanga')
               where Type = '64ac91ef-bdb3-4086-be17-bb1dbe7a7656'");

      var mainHosts = Repository.GetStateless<Services.MangaSetting>().Select(s => s.MainUri.Host).Distinct().ToList();
      using (var context = Repository.GetEntityContext())
      {
        foreach (var manga in context.Get<IManga>())
        {
          if (mainHosts.Contains(manga.Uri.Host))
            continue;

          var newHost = manga.Setting.MainUri.Host;
          var builder = new UriBuilder(manga.Uri) { Host = newHost, Port = -1 };
          manga.Uri = builder.Uri;
          context.Save(manga);
        }
      }
    }

    public From37To38()
    {
      this.Version = new Version(1, 37, 3);
    }
  }
}