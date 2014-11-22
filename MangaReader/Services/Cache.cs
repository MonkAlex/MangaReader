using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MangaReader.Manga;
using NHibernate.Linq;

namespace MangaReader.Services
{
  public static class Cache
  {
    /// <summary>
    /// Указатель блокировки файла истории.
    /// </summary>
    private static readonly object CacheLock = new object();

    /// <summary>
    /// Ссылка на файл лога.
    /// </summary>
    private static readonly string CacheFile = Settings.WorkFolder + @".\Cache";

    /// <summary>
    /// Добавление манги в кеш.
    /// </summary>
    /// <param name="mangas">Манга.</param>
    public static void Add(ObservableCollection<Mangas> mangas)
    {
      var session = Mapping.Environment.Session;
      foreach (var manga in mangas)
      {
        session.SaveOrUpdate(manga);
      }
    }

    /// <summary>
    /// Получить мангу из кеша.
    /// </summary>
    /// <returns>Манга.</returns>
    public static ObservableCollection<Mangas> Get()
    {
      var fromDb = Mapping.Environment.Session.Query<Mangas>().ToList();
      return new ObservableCollection<Mangas>(fromDb);
    }

    internal static void Convert(ConverterProcess process)
    {
      if (!File.Exists(CacheFile))
        return;

      var globalCollection = new List<Mangas>();

      var obsoleteManga = File.Exists(CacheFile) ?
// ReSharper disable once CSharpWarnings::CS0612
          Serializer<ObservableCollection<Manga.Manga>>.Load(CacheFile) :
          null;
      if (obsoleteManga != null)
      {
        globalCollection.AddRange(obsoleteManga.Select(manga => new Manga.Grouple.Readmanga()
        {
          Name = manga.Name,
          Url = manga.Url,
          Status = manga.Status,
          NeedUpdate = manga.NeedUpdate
        }));
      }

      var cache = File.Exists(CacheFile) ?
          Serializer<ObservableCollection<Mangas>>.Load(CacheFile).ToList() :
          new ObservableCollection<Mangas>(Enumerable.Empty<Mangas>()).ToList();
      globalCollection.AddRange(cache.Where(gm => !globalCollection.Exists(m => m.Url == gm.Url)));

      var session = Mapping.Environment.Session;
      var fileUrls = globalCollection.Select(m => m.Url).ToList();
      var dbMangas = session.Query<Mangas>().ToList();
      var fromFileInDb = dbMangas.Where(m => fileUrls.Contains(m.Url)).ToList();
      if (fromFileInDb.Count == 0)
        fromFileInDb = globalCollection.ToList();
      var onlyInDb = dbMangas.Where(m => !fileUrls.Contains(m.Url)).ToList();
      globalCollection = fromFileInDb.Concat(onlyInDb).ToList();

      process.IsIndeterminate = false;
      using (var tranc = session.BeginTransaction())
      {
        foreach (var manga in globalCollection)
        {
          process.Percent += 100.0 / globalCollection.Count;
          session.Save(manga);
        }
        tranc.Commit();
      }

      File.Move(CacheFile, CacheFile + ".dbak");
    }
  }
}
