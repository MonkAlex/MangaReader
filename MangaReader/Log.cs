using System;
using System.IO;
using System.Windows;

namespace MangaReader
{
    public static class Log
    {
        /// <summary>
        /// Ссылка на файл лога.
        /// </summary>
        private const string logPath = @".\manga.log";

        /// <summary>
        /// Уровень события.
        /// </summary>
        public enum Level
        {
            None,
            Information,
            Warning,
            Error
        }

        /// <summary>
        /// Добавление записи в лог.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <param name="logLevel">Уровень события.</param>
        public static void Add(string message, Level logLevel = Level.None)
        {
            if (logLevel == Level.Error)
                MessageBox.Show(message, message, MessageBoxButton.OK, MessageBoxImage.Error);

            if (logLevel == Level.Warning)
                MessageBox.Show(message, message, MessageBoxButton.OK, MessageBoxImage.Warning);

            if (logLevel == Level.Information)
                MessageBox.Show(message, message, MessageBoxButton.OK, MessageBoxImage.Information);
            
            File.AppendAllText(logPath, string.Concat(DateTime.Now, " ", message, Environment.NewLine), System.Text.Encoding.UTF8);
        }
    }
}
