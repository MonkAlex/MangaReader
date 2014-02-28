using System;
using System.IO;
using System.Windows;

namespace MangaReader
{
    public class Log
    {
        /// <summary>
        /// Указатель блокировки лог файла.
        /// </summary>
        private static object logLock = new object();

        /// <summary>
        /// Ссылка на файл лога.
        /// </summary>
        private static string logPath = @".\manga.log";

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
            lock (logLock)
            {
                File.AppendAllText(logPath,
                    string.Concat(DateTime.Now,
                        "   ",
                        message,
                        Environment.NewLine,
                        Environment.NewLine),
                    System.Text.Encoding.UTF8);
            }
        }

        public Log()
        {
            throw new Exception("Use static methods. Dont create log object.");
        }
    }
}
