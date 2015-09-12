using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MangaReader.Manga;
using NHibernate.Linq;

namespace MangaReader.Services
{
  [Obsolete("Класс нужен только для старых данных.")]
  public static class Cache
  {
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
#pragma warning disable CS0612 // Obsolete методы используются для конвертации
          Serializer<ObservableCollection<Manga.Manga>>.Load(CacheFile) :
#pragma warning restore CS0612
          null;
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

      if (File.Exists(CacheFile))
      {
        var cache = Serializer<ObservableCollection<Mangas>>.Load(CacheFile);
        if (cache != null)
          globalCollection.AddRange(cache.Where(gm => !globalCollection.Exists(m => m.Uri == gm.Uri)));
      }

      var session = Mapping.Environment.Session;
      var fileUrls = globalCollection.Select(m => m.Uri).ToList();
      var dbMangas = session.Query<Mangas>().ToList();
      var fromFileInDb = dbMangas.Where(m => fileUrls.Contains(m.Uri)).ToList();
      if (fromFileInDb.Count == 0)
        fromFileInDb = globalCollection.ToList();
      var onlyInDb = dbMangas.Where(m => !fileUrls.Contains(m.Uri)).ToList();
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

      Backup.MoveToBackup(CacheFile);
    }
  }
}
