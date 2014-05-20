using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace MangaReader
{
    class Library
    {
        /// <summary>
        /// Ссылка на файл базы.
        /// </summary>
        private static readonly string Database = Settings.WorkFolder + @".\db";

        /// <summary>
        /// Манга в библиотеке.
        /// </summary>
        public static ObservableCollection<Manga> DatabaseMangas = new ObservableCollection<Manga>();

        /// <summary>
        /// Получить мангу в базе.
        /// </summary>
        /// <returns>Манга.</returns>
        public static ObservableCollection<Manga> GetMangas()
        {
            if (!File.Exists(Database))
                return null;

            if (DatabaseMangas.Any())
                return DatabaseMangas;
            
            foreach (var line in File.ReadAllLines(Database))
            {
                DatabaseMangas.Add(new Manga(line));
            }
            Cache.Add(DatabaseMangas);
            return DatabaseMangas;
        }

        /// <summary>
        /// Обновить мангу.
        /// </summary>
        /// <param name="needCompress">Сжимать скачанное?</param>
        /// <param name="manga">Обновляемая манга. По умолчанию - вся.</param>
        public static void Update(Manga manga = null, bool needCompress = true)
        {
            Settings.Update = true;

            var mangas = manga == null ? GetMangas() : new ObservableCollection<Manga> { manga };

            foreach (var current in mangas)
            {
                var folder = Settings.DownloadFolder + "\\" + current.Name;
                current.Download(folder);
                if (needCompress)
                    Comperssion.ComperssVolumes(folder);
            }
        }

        public Library()
        {
            throw new Exception("Use methods.");
        }
    }
}
