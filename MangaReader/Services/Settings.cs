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
        public static readonly string WorkFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Настройки программы.
        /// </summary>
        private static readonly string SettingsPath = WorkFolder + "\\settings.xml";

        /// <summary>
        /// Автообновление программы.
        /// </summary>
        public static bool UpdateReader = true;

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
        /// Сжимать скачанную мангу.
        /// </summary>
        public static bool CompressManga = true;

        /// <summary>
        /// Сохранить настройки.
        /// </summary>
        public static void Save()
        {
            object[] settings = 
            {
                Language,
                Update,
                UpdateReader,
                DownloadFolder,
                CompressManga
            };
            Serializer<object[]>.Save(SettingsPath, settings);
        }

        /// <summary>
        /// Загрузить настройки.
        /// </summary>
        public static void Load()
        {
            var settings = Serializer<object[]>.Load(SettingsPath);
            if (settings == null)
                return;

            try
            {
                Language = (Languages) settings[0];
                Update = (bool) settings[1];
                UpdateReader = (bool) settings[2];
                DownloadFolder = (string) settings[3];
                CompressManga = (bool) settings[4];
            }
            catch (IndexOutOfRangeException){}
        }

        /// <summary>
        /// Доступные языки.
        /// </summary>
        public enum Languages
        {
            English,
            Russian,
            Japanese
        }

        public Settings(){}
    }
}
