using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MangaReader.Manga;

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
      using (var session = Mapping.Environment.SessionFactory.OpenSession())
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
      using (var session = Mapping.Environment.SessionFactory.OpenSession())
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
      using (var session = Mapping.Environment.SessionFactory.OpenSession())
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
      lock (CacheLock)
      {
        CachedMangas = CachedMangas ??
            (File.Exists(CacheFile) ?
            Serializer<ObservableCollection<Mangas>>.Load(CacheFile) :
            new ObservableCollection<Mangas>(Enumerable.Empty<Mangas>()));
      }
      using (var session = Mapping.Environment.SessionFactory.OpenSession())
      {
        var fileUrls = CachedMangas.Select(m => m.Url).ToList();
        var dbMangas = session.QueryOver<Mangas>().List();
        var fromFileInDb = dbMangas.Where(m => fileUrls.Contains(m.Url)).ToList();
        if (fromFileInDb.Count == 0)
          fromFileInDb = CachedMangas.ToList();
        var onlyInDb = dbMangas.Where(m => !fileUrls.Contains(m.Url)).ToList();
        CachedMangas = new ObservableCollection<Mangas>(fromFileInDb.Concat(onlyInDb).ToList());
      }
      return CachedMangas;
    }

    public static void Convert()
    {
      var newMangas = new ObservableCollection<Mangas>(Enumerable.Empty<Mangas>());
      lock (CacheLock)
      {
        var obsoleteManga = File.Exists(CacheFile) ?
            Serializer<ObservableCollection<Manga.Manga>>.Load(CacheFile) :
            null;
        if (obsoleteManga != null)
        {
          foreach (var manga in obsoleteManga)
          {
            newMangas.Add(new Manga.Grouple.Readmanga()
            {
              Name = manga.Name,
              Url = manga.Url,
              Status = manga.Status,
              NeedUpdate = manga.NeedUpdate
            });
          }
          // TODO: выпилить, перевести конвертирование сразу в базу.
          // Serializer<ObservableCollection<Mangas>>.Save(CacheFile, newMangas);
        }
      }

      foreach (var manga in newMangas)
      {
        manga.Save();
      }
    }

    public Cache()
    {
      throw new Exception("U shell not pass.");
    }
  }
}
