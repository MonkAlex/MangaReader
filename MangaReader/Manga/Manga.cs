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
        /// Закешированный список глав.
        /// </summary>
        private List<Chapter> allChapters = null;

        /// <summary>
        /// Список глав, ссылка-описание.
        /// </summary>
        private readonly Dictionary<string, string> listOfChapters;

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
        public void Download(string mangaFolder, string volumePrefix, string chapterPrefix)
        {
            Log.Add("Download start " + this.Name);
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

            // Формируем путь к главе вида Папка_манги\Том_001\Глава_0001
            try
            {
                Parallel.ForEach(newChapters, ch => ch.Download(string.Concat(mangaFolder,
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

        public Manga(string url)
        {
            this.Url = url;
            var page = Page.GetPage(url);
            this.Name = Getter.GetMangaName(page).ToString();
            this.listOfChapters = Getter.GetLinksOfMangaChapters(page, url);
        }
    }
}
