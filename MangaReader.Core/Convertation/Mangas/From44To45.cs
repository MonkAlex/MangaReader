using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation.Mangas
{
  public class From44To45 : MangasConverter
  {
    private List<MangaProxy> mangaCreated = new List<MangaProxy>();

    private bool firstRun = true;

    private bool hasEmptyRecords = false;

    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) &&
             this.Version.CompareTo(Repository.GetStateless<DatabaseConfig>().Single().Version) > 0 &&
             process.Version.CompareTo(this.Version) >= 0;
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      process.Percent = 0;
      using (var context = Repository.GetEntityContext())
      {
        using (var tranc = context.OpenTransaction())
        {
          var mangas = context.Get<IManga>().OrderBy(m => m.Id).ToList();
          foreach (var manga in mangas)
          {
            process.Percent += 100.0 / mangas.Count;
            DateTime? min = null;
            if (firstRun && manga.Histories.Any())
              min = manga.Histories.Min(h => h.Date);
            if (firstRun && min == null)
            {
              var folder = new System.IO.DirectoryInfo(manga.GetAbsoulteFolderPath());
              if (folder.Exists)
                min = folder.CreationTime;
            }
            if (!firstRun && min == null)
            {
              if (!mangaCreated.Any())
                min = GetDefaultDate();
              if (min == null)
              {
                var before = mangaCreated.TakeWhile(m => m.Id < manga.Id).LastOrDefault();
                var after = mangaCreated.SkipWhile(m => m.Id < manga.Id).FirstOrDefault();
                if (before == null && after == null)
                  min = GetDefaultDate();
                else if (before == null)
                  min = after.Created - TimeSpan.FromSeconds(after.Id - manga.Id);
                else if (after == null)
                  min = before.Created + TimeSpan.FromSeconds(manga.Id - before.Id);
                else
                {
                  var allowedSeconds = (after.Created - before.Created).TotalSeconds;
                  var hasIds = after.Id - before.Id;
                  var period = allowedSeconds / hasIds;
                  min = before.Created.AddSeconds(period * (manga.Id - before.Id));
                }
              }
              Log.Add($"Манге {manga.Name} не удалось найти примерную дату добавления, проставлена {min}.");
            }
            if (min != null)
              mangaCreated.Add(new MangaProxy(manga.Id, min.Value));
            else
              hasEmptyRecords = true;
            if (manga.Created == null || manga.Created > min)
              manga.Created = min;
            context.SaveOrUpdate(manga);
          }
          tranc.Commit();
        }
      }

      if (hasEmptyRecords)
      {
        hasEmptyRecords = false;
        firstRun = false;
        ProtectedConvert(process);
      }
    }

    private static DateTime GetDefaultDate()
    {
      return DateTime.Today.AddDays(-1);
    }

    public From44To45()
    {
      this.Version = new Version(1, 44, 11);
      this.CanReportProcess = true;
      this.Name = "Определение даты добавления манги в библиотеку...";
    }

    private class MangaProxy
    {
      public int Id;
      public DateTime Created;

      public MangaProxy(int id, DateTime created)
      {
        this.Id = id;
        this.Created = created;
      }
    }
  }
}