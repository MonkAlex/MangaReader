using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MangaReader.Manga;
using MangaReader.Manga.Grouple;


namespace MangaReader
{
    class Cache
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
            lock (CacheLock)
                Serializer <ObservableCollection<Mangas>>.Save(CacheFile, CachedMangas);
        }

        /// <summary>
        /// Добавление манги в кеш.
        /// </summary>
        /// <param name="mangas">Манга.</param>
        public static void Add(ObservableCollection<Mangas> mangas)
        {
            lock (CacheLock)
                CachedMangas = mangas;
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
                return CachedMangas;
            }
        }

        public static void Convert()
        {
            lock (CacheLock)
            {
                var obsoleteManga = File.Exists(CacheFile) ?
                    Serializer<ObservableCollection<Manga.Manga>>.Load(CacheFile) :
                    null;
                if (obsoleteManga != null)
                {
                    var newMangas = new ObservableCollection<Mangas>(Enumerable.Empty<Mangas>());
                    foreach (var manga in obsoleteManga)
                    {
                        newMangas.Add(new Readmanga()
                        {
                            Name = manga.Name,
                            Url = manga.Url,
                            Status = manga.Status,
                            NeedUpdate = manga.NeedUpdate
                        });
                    }
                    Serializer<ObservableCollection<Mangas>>.Save(CacheFile, newMangas);
                }
            }
        }

        public Cache()
        {
            throw new Exception("U shell not pass.");
        }
    }
}
