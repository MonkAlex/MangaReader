using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;


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

        private static ObservableCollection<Manga> CachedMangas;

        /// <summary>
        /// Сохранить кеш на диск.
        /// </summary>
        public static void Save()
        {
            lock (CacheLock)
                Serializer<ObservableCollection<Manga>>.Save(CacheFile, CachedMangas);
        }

        /// <summary>
        /// Добавление манги в кеш.
        /// </summary>
        /// <param name="mangas">Манга.</param>
        public static void Add(ObservableCollection<Manga> mangas)
        {
            lock (CacheLock)
              CachedMangas = mangas;
        }

        /// <summary>
        /// Получить мангу из кеша.
        /// </summary>
        /// <returns>Манга.</returns>
        public static ObservableCollection<Manga> Get()
        {
            lock (CacheLock)
            {
                return CachedMangas ??
                    (File.Exists(CacheFile) ?
                    Serializer<ObservableCollection<Manga>>.Load(CacheFile) :
                    new ObservableCollection<Manga>(Enumerable.Empty<Manga>()));
            }
        }

        public Cache()
        {
            throw new Exception("U shell not pass.");
        }
    }
}
