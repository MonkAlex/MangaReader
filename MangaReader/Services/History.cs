using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MangaReader
{
    class History
    {
        /// <summary>
        /// Указатель блокировки файла истории.
        /// </summary>
        private static readonly object HistoryLock = new object();

        /// <summary>
        /// Ссылка на файл лога.
        /// </summary>
        private static readonly string HistoryPath = Settings.WorkFolder + @".\history";

        /// <summary>
        /// Добавление записи в историю.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public static void Add(string message)
        {
            if (!Contains(message))
                lock (HistoryLock)
                    File.AppendAllText(HistoryPath, string.Concat(message, Environment.NewLine), System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Получить историю.
        /// </summary>
        /// <param name="subString">Подстрока, по которой будет получена история. Например, название манги.</param>
        /// <returns>Перечисление сообщений из истории.</returns>
        public static IEnumerable<string> Get(string subString = "")
        {
            IEnumerable<string> history = new string[] {};
            lock (HistoryLock)
                if (File.Exists(HistoryPath))
                    history = File.ReadAllLines(HistoryPath);
            if (subString != string.Empty)
            {
                history = history.Where(l => CultureInfo
                    .InvariantCulture
                    .CompareInfo
                    .IndexOf(l, subString, CompareOptions.IgnoreCase) >= 0);
            }
            return history;
        }

        /// <summary>
        /// Проверка наличия сообщения в истории.
        /// </summary>
        /// <param name="message">Сообщение для проверки.</param>
        /// <returns>True, если сообщение уже есть в истории.</returns>
        public static bool Contains(string message)
        {
            IEnumerable<string> history = new string[] { };
            lock (HistoryLock)
                if (File.Exists(HistoryPath))
                    history = File.ReadAllLines(HistoryPath);
            if (message != null)
                return history.Any(m => CultureInfo
                    .InvariantCulture
                    .CompareInfo
                    .Compare(m, message, CompareOptions.IgnoreCase) == 0);
            return false;
        }

        /// <summary>
        /// Исключить сообщения имеющиеся в истории.
        /// </summary>
        /// <param name="messages">Сообщения для проверки.</param>
        /// <returns>Сообщения, не найденные в истории. Null, если сообщения для проверки не были переданы.</returns>
        public static IEnumerable<string> Except(IEnumerable<string> messages)
        {
            IEnumerable<string> history = new string[] { };
            lock (HistoryLock)
                if (File.Exists(HistoryPath))
                    history = File.ReadAllLines(HistoryPath);
            return messages != null ?
                messages.Where(m => !history.Contains(m, StringComparer.InvariantCultureIgnoreCase)) :
                null;
        }

        public History()
        {
            throw new Exception("U shell not pass.");
        }
    }
}
