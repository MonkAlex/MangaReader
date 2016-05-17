using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using NHibernate.Linq;

namespace MangaReader.Core.Convertation.Mangas
{
  public class FromBaseTo24Db : MangasConverter
  {
    /// <summary>
    /// Ссылка на файл базы.
    /// </summary>
    private static readonly string DatabaseFile = Path.Combine(ConfigStorage.WorkFolder, "db");

    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && File.Exists(DatabaseFile);
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var database = Serializer<List<string>>.Load(DatabaseFile) ?? new List<string>(File.ReadAllLines(DatabaseFile));
      var mangaUrls = Mapping.Session.Query<Manga.Mangas>().Select(m => m.Uri.ToString()).ToList();
      database = database.Where(s => !mangaUrls.Contains(s)).ToList();

      if (database.Any())
        process.ProgressState = ProgressState.Normal;

      foreach (var dbstring in database)
      {
        Manga.Mangas.Create(new Uri(dbstring));
        process.Percent += 100.0 / database.Count;
      }

      Backup.MoveToBackup(DatabaseFile);
    }

    public FromBaseTo24Db()
    {
      this.Version = new Version(1, 0, 1);
      this.CanReportProcess = true;
    }
  }
}