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
        private static object historyLock = new object();

        /// <summary>
        /// Ссылка на файл лога.
        /// </summary>
        private static string historyPath = @".\history";

        /// <summary>
        /// Добавление записи в историю.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public static void Add(string message)
        {
            if (!Contains(message))
                lock (historyLock)
                    File.AppendAllText(historyPath, string.Concat(message, Environment.NewLine), System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Получить историю.
        /// </summary>
        /// <param name="subString">Подстрока, по которой будет получена история. Например, название манги.</param>
        /// <returns>Перечисление сообщений из истории.</returns>
        public static IEnumerable<string> Get(string subString = "")
        {
            IEnumerable<string> history = new string[] {};
            lock (historyLock)
                if (File.Exists(historyPath))
                    history = File.ReadAllLines(historyPath);
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
            lock (historyLock)
                if (File.Exists(historyPath))
                    history = File.ReadAllLines(historyPath);
            if (message != null)
                return history.Where(m => CultureInfo
                    .InvariantCulture
                    .CompareInfo
                    .Compare(m, message, CompareOptions.IgnoreCase) == 0)
                    .Any();
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
            lock (historyLock)
                if (File.Exists(historyPath))
                    history = File.ReadAllLines(historyPath);
            if (messages != null)
                return messages.Where(m => !history.Contains(m, StringComparer.InvariantCultureIgnoreCase));
            return null;
        }

        public History()
        {
            throw new Exception("U shell not pass.");
        }
    }
}
