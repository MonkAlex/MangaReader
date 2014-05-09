using System;
using System.IO;
using System.Reflection;

namespace MangaReader
{
    public class Settings
    {
        /// <summary>
        /// Язык манги.
        /// </summary>
        public static Languages Language = Languages.English;

        /// <summary>
        /// Обновлять при скачивании (true) или скачивать целиком(false).
        /// </summary>
        public static bool Update = true;

        /// <summary>
        /// Папка программы.
        /// </summary>
        public static string WorkFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Папка загрузки.
        /// </summary>
        public static string DownloadFolder = WorkFolder + "\\Download\\";

        /// <summary>
        /// Префикс папки томов.
        /// </summary>
        public static string VolumePrefix = "Volume_";

        /// <summary>
        /// Префикс папки глав.
        /// </summary>
        public static string ChapterPrefix = "Chapter_";


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
            throw new Exception("Use properties.");
        }
    }
}
