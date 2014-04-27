using System;

namespace MangaReader
{
    public class Settings
    {
        /// <summary>
        /// Язык манги.
        /// </summary>
        public static Languages Language { get; set; }

        /// <summary>
        /// Обновлять при скачивании (true) или скачивать целиком(false).
        /// </summary>
        public static bool Update { get; set; }

        /// <summary>
        /// Папка программы.
        /// </summary>
        public static string WorkFolder { get; set; }

        /// <summary>
        /// Доступные языки.
        /// </summary>
        public enum Languages
        {
            English,
            Russian,
            Japanese
        }

        public Settings()
        {
            throw new Exception("Use methods.");
        }
    }
}
