using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;


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
        private static readonly string CachePath = Settings.WorkFolder + @".\Cache\";

        /// <summary>
        /// Добавление манги в кеш.
        /// </summary>
        /// <param name="manga">Манга.</param>
        public static void Add(Manga manga)
        {
            lock (CacheLock)
                Serializer<Manga>.Save(CachePath + manga.Name, manga);
        }

        /// <summary>
        /// Добавление манги в кеш.
        /// </summary>
        /// <param name="mangas">Манга.</param>
        public static void Add(ObservableCollection<Manga> mangas)
        {
            lock (CacheLock)
                foreach (var manga in mangas)
                {
                    Serializer<Manga>.Save(CachePath + manga.Name, manga);
                }
        }

        /// <summary>
        /// Получить мангу из кеша.
        /// </summary>
        /// <param name="name">Название манги.</param>
        /// <returns>Манга.</returns>
        public static Manga Get(string name)
        {
            lock (CacheLock)
                if (File.Exists(CachePath + name))
                    return Serializer<Manga>.Load(CachePath + name);
            return null;
        }

        /// <summary>
        /// Получить мангу из кеша.
        /// </summary>
        /// <returns>Манга.</returns>
        public static ObservableCollection<Manga> Get()
        {
            lock (CacheLock)
            {
                if (!Directory.Exists(CachePath))
                    return null;
                var mangas = new ObservableCollection<Manga>();
                foreach (var file in Directory.GetFiles(CachePath))
                {
                    mangas.Add(Serializer<Manga>.Load(file));
                }
                return mangas;
            }
        }

        public Cache()
        {
            throw new Exception("U shell not pass.");
        }
    }

    public static class Serializer<T>
    {
        public static void Save(string path, T data)
        {
            var formatter = new XmlSerializer(typeof(T));

            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                formatter.Serialize(stream, data);
            }
        }

        public static T Load(string path)
        {
            Type type = typeof(T);
            T retVal;

            var formatter = new XmlSerializer(type);

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                retVal = (T)formatter.Deserialize(stream);
            }

            return retVal;
        }
    }
}
