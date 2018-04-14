using System;
using System.Linq;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;

namespace MangaReader.Core.Convertation.Mangas
{
  public class From44To45DownloadedAt : MangasConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && this.CanConvertVersion(process);
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      process.Percent = 0;
      using (var context = Repository.GetEntityContext())
      {
        using (var tranc = context.OpenTransaction())
        {
          var mangas = context.Get<IManga>().Where(m => m.DownloadedAt == null).OrderBy(m => m.Id).ToList();
          foreach (var manga in mangas)
          {
            process.Percent += 100.0 / mangas.Count;
            DateTime? downloaded = null;
            if (manga.Histories.Any())
              downloaded = manga.Histories.Max(h => h.Date);
            if (downloaded == null)
              Log.Add($"Манге {manga.Name} не удалось найти примерную дату скачивания.");
            if (manga.DownloadedAt == null || manga.DownloadedAt < downloaded)
              manga.DownloadedAt = downloaded;
            context.SaveOrUpdate(manga);
          }
          tranc.Commit();
        }
      }
    }

    public From44To45DownloadedAt()
    {
      this.Version = new Version(1, 44, 12);
      this.CanReportProcess = true;
      this.Name = "Поиск примерной даты скачивания манги...";
    }
  }
}