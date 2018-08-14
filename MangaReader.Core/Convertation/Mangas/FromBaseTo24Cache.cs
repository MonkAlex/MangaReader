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

namespace MangaReader.Core.Convertation.Mangas
{
  public class FromBaseTo24Cache : MangasConverter
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

      var globalCollection = new List<Manga.IManga>();

      var cache = Serializer<ObservableCollection<Manga.Mangas>>.Load(CacheFile);
      if (cache != null)
        globalCollection.AddRange(cache.Where(gm => !globalCollection.Exists(m => m.Uri == gm.Uri)));

      var fileUrls = globalCollection.Select(m => m.Uri).ToList();
      var settings = Repository.GetStateless<MangaSetting>();
      using (var context = Repository.GetEntityContext())
      {
        var dbMangas = context.Get<IManga>().ToList();
        var fromFileInDb = dbMangas.Where(m => fileUrls.Contains(m.Uri)).ToList();
        if (fromFileInDb.Count == 0)
          fromFileInDb = globalCollection.ToList();
        var onlyInDb = dbMangas.Where(m => !fileUrls.Contains(m.Uri)).ToList();
        globalCollection = fromFileInDb.Concat(onlyInDb).ToList();
        foreach (var manga in globalCollection.Where(m => m.Setting == null).OfType<Manga.Mangas>())
          manga.Setting = settings.Single(s => s.Manga == ConfigStorage.GetPlugin(manga.GetType()).MangaGuid);
        globalCollection.SaveAll(context);
      }

      Backup.MoveToBackup(CacheFile);
    }

    public FromBaseTo24Cache()
    {
      this.Version = new Version(1, 0, 0);
    }
  }
}
