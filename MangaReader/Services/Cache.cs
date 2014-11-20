using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MangaReader.Manga;
using NHibernate.Mapping;

namespace MangaReader.Services
{
  public class Cache
  {
    /// <summary>
    /// Указатель блокировки файла истории.
    /// </summary>
    private static readonly object CacheLock = new object();

    /// <summary>
    /// Ссылка на файл лога.
    /// </summary>
    private static readonly string CacheFile = Settings.WorkFolder + @".\Cache";

    private static ObservableCollection<Mangas> CachedMangas;

    /// <summary>
    /// Сохранить кеш на диск.
    /// </summary>
    public static void Save()
    {
      using (var session = Mapping.Environment.OpenSession())
      {
        foreach (var manga in CachedMangas)
        {
          session.SaveOrUpdate(manga);
        }
      }
    }

    /// <summary>
    /// Добавление манги в кеш.
    /// </summary>
    /// <param name="mangas">Манга.</param>
    public static void Add(ObservableCollection<Mangas> mangas)
    {
      lock (CacheLock)
        CachedMangas = mangas;
      using (var session = Mapping.Environment.OpenSession())
      {
        foreach (var manga in mangas)
        {
          session.SaveOrUpdate(manga);
        }
      }
    }

    /// <summary>
    /// Получить мангу из кеша.
    /// </summary>
    /// <param name="id">Id манги.</param>
    /// <returns>Манга.</returns>
    public static Mangas Get(int id)
    {
      Mangas manga;
      using (var session = Mapping.Environment.OpenSession())
      {
        manga = session.Get<Mangas>(id);
      }
      return manga;
    }

    /// <summary>
    /// Получить мангу из кеша.
    /// </summary>
    /// <returns>Манга.</returns>
    public static ObservableCollection<Mangas> Get()
    {
      using (var session = Mapping.Environment.OpenSession())
      {
        return new ObservableCollection<Mangas>(session.QueryOver<Mangas>().List());
      }
    }

    internal static void Convert(ConverterProcess process)
    {
      var globalCollection = new List<Mangas>();

      lock (CacheLock)
      {
        var obsoleteManga = File.Exists(CacheFile) ?
            Serializer<ObservableCollection<Manga.Manga>>.Load(CacheFile) :
            null;
        if (obsoleteManga != null)
        {
          globalCollection.AddRange(obsoleteManga.Select(manga => new Manga.Grouple.Readmanga()
          {
            Name = manga.Name, Url = manga.Url, Status = manga.Status, NeedUpdate = manga.NeedUpdate
          }));
        }
      }

      lock (CacheLock)
      {
        var cache = File.Exists(CacheFile) ?
            Serializer<ObservableCollection<Mangas>>.Load(CacheFile).ToList() :
            new ObservableCollection<Mangas>(Enumerable.Empty<Mangas>()).ToList();
        globalCollection.AddRange(cache.Where(gm => !globalCollection.Exists(m => m.Url == gm.Url)));
      }
      using (var session = Mapping.Environment.OpenSession())
      {
        var fileUrls = globalCollection.Select(m => m.Url).ToList();
        var dbMangas = session.QueryOver<Mangas>().List();
        var fromFileInDb = dbMangas.Where(m => fileUrls.Contains(m.Url)).ToList();
        if (fromFileInDb.Count == 0)
          fromFileInDb = globalCollection.ToList();
        var onlyInDb = dbMangas.Where(m => !fileUrls.Contains(m.Url)).ToList();
        globalCollection = fromFileInDb.Concat(onlyInDb).ToList();
      }

      process.IsIndeterminate = false;
      using (var session = Mapping.Environment.OpenSession())
      using (var tranc = session.BeginTransaction())
      {
        foreach (var manga in globalCollection)
        {
          process.Percent += 100.0/globalCollection.Count;
          session.Save(manga);
        }
        tranc.Commit();
      }
    }

    public Cache()
    {
      throw new Exception("U shell not pass.");
    }
  }
}
