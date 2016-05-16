using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation
{
  public class CacheConverter : BaseConverter
  {
    /// <summary>
    /// Ссылка на файл лога.
    /// </summary>
    private static readonly string CacheFile = ConfigStorage.WorkFolder + @".\Cache";

    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && File.Exists(CacheFile);
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      Log.AddFormat("Get {0} manga settings before manga converts.:", ConfigStorage.Instance.DatabaseConfig.MangaSettings.Count);
      var globalCollection = new List<Mangas>();

#pragma warning disable CS0612 // Obsolete методы используются для конвертации
      var obsoleteManga = Serializer<ObservableCollection<Manga.Manga>>.Load(CacheFile);
#pragma warning restore CS0612
      if (obsoleteManga != null)
      {
        globalCollection.AddRange(obsoleteManga.Select(manga => new Manga.Grouple.Readmanga()
        {
          Name = manga.Name,
          Uri = new Uri(manga.Url),
          Status = manga.Status,
          NeedUpdate = manga.NeedUpdate
        }));
      }

      var cache = Serializer<ObservableCollection<Mangas>>.Load(CacheFile);
      if (cache != null)
        globalCollection.AddRange(cache.Where(gm => !globalCollection.Exists(m => m.Uri == gm.Uri)));

      var fileUrls = globalCollection.Select(m => m.Uri).ToList();
      var dbMangas = Repository.Get<Mangas>().ToList();
      var fromFileInDb = dbMangas.Where(m => fileUrls.Contains(m.Uri)).ToList();
      if (fromFileInDb.Count == 0)
        fromFileInDb = globalCollection.ToList();
      var onlyInDb = dbMangas.Where(m => !fileUrls.Contains(m.Uri)).ToList();
      globalCollection = fromFileInDb.Concat(onlyInDb).ToList();
      globalCollection.SaveAll();

      Backup.MoveToBackup(CacheFile);
    }
  }
}
