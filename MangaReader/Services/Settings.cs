using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using MangaReader.Logins;

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
        /// Состояние окна.
        /// </summary>
        public static object[] WindowsState;

        /// <summary>
        /// Логин.
        /// </summary>
        public static Login Login = new Login();

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
                CompressManga,
                WindowsState,
                new object[] {Login.Name, Login.Password}
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
                WindowsState = (object[]) settings[5];
                Login = new Login() { Name = (string)((object[])settings[6])[0], Password = (string)((object[])settings[6])[1] }; 
            }
            catch (IndexOutOfRangeException){}
        }

        /// <summary>
        /// Загрузить положение и размеры окна.
        /// </summary>
        /// <param name="main">Окно.</param>
        public static void UpdateWindowsState(MainWindow main)
        {
            if (WindowsState == null)
                return;
            try
            {
                main.Top = (double)WindowsState[0];
                main.Left = (double)WindowsState[1];
                main.Width = (double)WindowsState[2];
                main.Height = (double)WindowsState[3];
                main.WindowState = (WindowState)WindowsState[4];
            }
            catch (IndexOutOfRangeException) { }
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
