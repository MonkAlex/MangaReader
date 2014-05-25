using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

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
        public static ObservableCollection<Manga> DatabaseMangas = new ObservableCollection<Manga>(Enumerable.Empty<Manga>());

        /// <summary>
        /// Служба управления UI главного окна.
        /// </summary>
        private static Dispatcher formDispatcher;

        #region Методы

        /// <summary>
        /// Инициализация библиотеки - заполнение массива из кеша.
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<Manga> Initialize()
        {
            foreach (var manga in Cache.Get())
            {
                DatabaseMangas.Add(manga);
            }
            DatabaseMangas.CollectionChanged += (s, e) => Cache.Add(DatabaseMangas);
            formDispatcher = Dispatcher.CurrentDispatcher;
            return DatabaseMangas;
        }

        /// <summary>
        /// Добавить мангу.
        /// </summary>
        /// <param name="url"></param>
        public static void Add(string url)
        {
            IEnumerable<string> database = new string[] { };
            var urlExist = false;
            if (File.Exists(Database))
                database = File.ReadAllLines(Database);
            if (!string.IsNullOrWhiteSpace(url))
                urlExist = database.Any(m => CultureInfo
                    .InvariantCulture
                    .CompareInfo
                    .Compare(m, url, CompareOptions.IgnoreCase) == 0);

            if (urlExist)
                return;

            var newManga = new Manga(url);
            if (!newManga.IsValid)
                return;

            File.AppendAllLines(Database, new[] {url});
            formDispatcher.Invoke(() => DatabaseMangas.Add(newManga));
        }

        /// <summary>
        /// Получить мангу в базе.
        /// </summary>
        /// <returns>Манга.</returns>
        public static ObservableCollection<Manga> GetMangas()
        {
            if (!File.Exists(Database))
                return null;

            foreach (var line in File.ReadAllLines(Database))
            {
                var manga = DatabaseMangas != null ? DatabaseMangas.FirstOrDefault(m => m.Url == line) : null;
                if (manga == null)
                {
                    var newManga = new Manga(line);
                    formDispatcher.Invoke(() => DatabaseMangas.Add(newManga));
                }
                else
                {
                    var index = DatabaseMangas.IndexOf(manga);
                    manga.Refresh();
                    formDispatcher.Invoke(() =>
                    {
                        DatabaseMangas.RemoveAt(index);
                        DatabaseMangas.Insert(index, manga);
                    });
                }
            }
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

            Parallel.ForEach(mangas, current =>
            {
                var folder = Settings.DownloadFolder + "\\" + current.Name;
                current.Download(folder);
                if (needCompress)
                    Comperssion.ComperssVolumes(folder);
            });
        }

        #endregion


        public Library()
        {
            throw new Exception("Use methods.");
        }
    }
}
