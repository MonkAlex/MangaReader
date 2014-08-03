using System;
using System.IO;

namespace MangaReader
{
    public class Log
    {
        /// <summary>
        /// Указатель блокировки лог файла.
        /// </summary>
        private static readonly object LogLock = new object();

        /// <summary>
        /// Указатель блокировки лог файла исключений.
        /// </summary>
        private static readonly object ExceptionLock = new object();

        /// <summary>
        /// Ссылка на файл лога.
        /// </summary>
        private static readonly string LogPath = Settings.WorkFolder + @".\manga.log";

        /// <summary>
        /// Ссылка на файл лога исключений.
        /// </summary>
        private static readonly string ExceptionPath = Settings.WorkFolder + @".\error.log";

        /// <summary>
        /// Добавление записи в лог.
        /// </summary>
        /// <param name="messages">Сообщение.</param>
        public static void Add(params object[] messages)
        {
            lock (LogLock)
            {
                File.AppendAllText(LogPath,
                    string.Concat(DateTime.Now,
                        "   ",
                        string.Join(Environment.NewLine, messages),
                        Environment.NewLine),
                    System.Text.Encoding.UTF8);
            }
        }

        /// <summary>
        /// Добавление записи в лог.
        /// </summary>
        /// <param name="ex">Исключение.</param>
        /// <param name="messages">Сообщение.</param>
        public static void Exception(Exception ex, params string[] messages)
        {
            lock (ExceptionLock)
            {
                File.AppendAllText(ExceptionPath,
                    string.Concat(DateTime.Now,
                        "   ",
                        string.Join(Environment.NewLine, messages),
                        Environment.NewLine,
                        ex.ToString(),
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
