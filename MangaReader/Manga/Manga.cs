using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangaReader
{
    /// <summary>
    /// Манга.
    /// </summary>
    public class Manga
    {
        #region Свойства

        /// <summary>
        /// Название манги.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Ссылка на мангу.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Статус перевода.
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// Обложка.
        /// </summary>
        public byte[] Cover { get; set; }

        /// <summary>
        /// Закешированный список глав.
        /// </summary>
        private List<Chapter> allChapters;

        /// <summary>
        /// Список глав, ссылка-описание.
        /// </summary>
        private Dictionary<string, string> listOfChapters;

        #endregion

        #region Методы

        /// <summary>
        /// Обновить информацию о манге - название, главы, обложка.
        /// </summary>
        public void Refresh()
        {
            var page = Page.GetPage(this.Url);
            this.Name = Getter.GetMangaName(page).ToString();
            this.listOfChapters = Getter.GetLinksOfMangaChapters(page, this.Url);
            this.Cover = Getter.GetMangaCover(page);
            Cache.Add(this);
        }

        /// <summary>
        /// Получить главу.
        /// </summary>
        /// <param name="chapterUrl">Ссылка на главу.</param>
        /// <returns>Глава.</returns>
        public Chapter GetChapter(string chapterUrl)
        {
            return listOfChapters
                .Where(ch => ch.Key == chapterUrl)
                .Select(ch => new Chapter(ch.Key, ch.Value))
                .FirstOrDefault();
        }

        /// <summary>
        /// Получить список глав.
        /// </summary>
        /// <returns>Список глав.</returns>
        public List<Chapter> GetAllChapters()
        {
            return allChapters ??
                   (allChapters = listOfChapters.Select(link => new Chapter(link.Key, link.Value)).ToList());
        }

        /// <summary>
        /// Скачать все главы.
        /// </summary>
        public void Download(string mangaFolder, string volumePrefix = null, string chapterPrefix = null)
        {
            if (volumePrefix == null)
                volumePrefix = Settings.VolumePrefix;
            if (chapterPrefix == null)
                chapterPrefix = Settings.ChapterPrefix;

            if (allChapters == null)
                GetAllChapters();

            var newChapters = allChapters;
            if (Settings.Update == true)
            {
                var messages = History.Get(this.Url);
                newChapters = newChapters
                    .Where(ch => !messages.Contains(ch.Url))
                    .ToList();
            }

            if (!newChapters.Any())
                return;

            Log.Add("Download start " + this.Name);

            // Формируем путь к главе вида Папка_манги\Том_001\Глава_0001
            try
            {
                Parallel.ForEach(newChapters,
                    ch => ch.Download(string.Concat(mangaFolder,
                        "\\",
                        volumePrefix,
                        ch.Volume.ToString().PadLeft(3, '0'),
                        "\\",
                        chapterPrefix,
                        ch.Number.ToString().PadLeft(4, '0')
                        )));
                Log.Add("Download end " + this.Name);
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
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Открыть мангу.
        /// </summary>
        /// <param name="url">Ссылка на мангу.</param>
        public Manga(string url)
        {
            this.Url = url;
            this.Refresh();
        }

        #endregion

    }
}
