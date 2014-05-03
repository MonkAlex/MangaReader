using System;
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
        /// Обновить мангу.
        /// </summary>
        /// <param name="needCompress">Сжимать скачанное?</param>
        public static void Update(bool needCompress = true)
        {
            Settings.Update = true;

            if (!File.Exists(Database))
                return;

            var links = File.ReadAllLines(Database);
            foreach (var manga in links.Select(link => new Manga(link)))
            {
                var folder = Settings.DownloadFolder + "\\" + manga.Name;
                manga.Download(folder, "Volume_", "Chapter_");
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
