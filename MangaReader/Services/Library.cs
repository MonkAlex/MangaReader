using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using MangaReader.Properties;

namespace MangaReader
{
    class Library
    {
        /// <summary>
        /// Ссылка на файл базы.
        /// </summary>
        private static readonly string DatabaseFile = Settings.WorkFolder + @".\db";

        /// <summary>
        /// База манги.
        /// </summary>
        private static List<string> Database = Serializer<List<string>>.Load(DatabaseFile);

        /// <summary>
        /// Манга в библиотеке.
        /// </summary>
        public static ObservableCollection<Manga> DatabaseMangas = new ObservableCollection<Manga>(Enumerable.Empty<Manga>());

        /// <summary>
        /// Статус библиотеки.
        /// </summary>
        public static string Status = string.Empty;

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
            if (Database.Contains(url))
                return;

            var newManga = new Manga(url);
            if (!newManga.IsValid)
                return;

            Database.Add(url);
            formDispatcher.Invoke(() => DatabaseMangas.Add(newManga));
            Status = Strings.Library_Status_MangaAdded + newManga.Name;
        }

        /// <summary>
        /// Удалить мангу.
        /// </summary>
        /// <param name="manga"></param>
        public static void Remove(Manga manga)
        {
            if (manga == null)
                return;

            Database.Remove(manga.Url);
            formDispatcher.Invoke(() => DatabaseMangas.Remove(manga));
            Status = Strings.Library_Status_MangaRemoved + manga.Name;
        }

        /// <summary>
        /// Сохранить библиотеку.
        /// </summary>
        public static void Save()
        {
            Serializer<List<string>>.Save(DatabaseFile, Database);
        }

        /// <summary>
        /// Сконвертировать в новый формат.
        /// </summary>
        public static void Convert()
        {
            if (Database != null)
                return;

            Database = File.Exists(DatabaseFile) ? new List<string>(File.ReadAllLines(DatabaseFile)) : new List<string>();
            Save();
        }
        /// <summary>
        /// Получить мангу в базе.
        /// </summary>
        /// <returns>Манга.</returns>
        public static ObservableCollection<Manga> GetMangas()
        {
            foreach (var line in Database)
                UpdateMangaByUrl(line);

            return DatabaseMangas;
        }

        /// <summary>
        /// Обновить состояние манги в библиотеке.
        /// </summary>
        /// <param name="line">Ссылка на мангу.</param>
        private static void UpdateMangaByUrl(string line)
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

        /// <summary>
        /// Обновить мангу.
        /// </summary>
        /// <param name="manga">Обновляемая манга. По умолчанию - вся.</param>
        public static void Update(Manga manga = null)
        {
            Settings.Update = true;

            ObservableCollection<Manga> mangas;
            if (manga != null)
            {
                UpdateMangaByUrl(manga.Url);
                mangas = new ObservableCollection<Manga> { manga };
            }
            else
            {
                Status = Strings.Library_Status_Update;
                mangas = GetMangas();
            }

            try
            {
                foreach (var current in mangas)
                {
                    Status = Strings.Library_Status_MangaUpdate + current.Name;
                    current.Download();
                    if (Settings.CompressManga)
                        Comperssion.ComperssVolumes(current.Folder);
                }
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                    Log.Exception(ex);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            finally
            {
                Status = Strings.Library_Status_UpdateComplete;
            }

        }

        #endregion


        public Library()
        {
            throw new Exception("Use methods.");
        }
    }
}
