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
        /// Указатель блокировки истории.
        /// </summary>
        private static readonly object HistoryLock = new object();

        /// <summary>
        /// Ссылка на файл лога.
        /// </summary>
        private static readonly string HistoryPath = Settings.WorkFolder + @".\history";

        /// <summary>
        /// История.
        /// </summary>
        private static List<string> Historis = Serializer<List<string>>.Load(HistoryPath);

        /// <summary>
        /// Добавление записи в историю.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public static void Add(string message)
        {
            if (Historis.Contains(message))
                return;

            lock (HistoryLock)
                Historis.Add(message);
        }

        /// <summary>
        /// Сохранить историю локально.
        /// </summary>
        public static void Save()
        {
            Serializer<List<string>>.Save(HistoryPath, Historis);
        }

        /// <summary>
        /// Сконвертировать в новый формат.
        /// </summary>
        public static void Convert()
        {
            if (Historis != null)
                return;

            Historis = new List<string>(File.ReadAllLines(HistoryPath));
            Save();
        }

        /// <summary>
        /// Получить историю.
        /// </summary>
        /// <param name="subString">Подстрока, по которой будет получена история. Например, название манги.</param>
        /// <returns>Перечисление сообщений из истории.</returns>
        public static IEnumerable<string> Get(string subString = "")
        {
            if (string.IsNullOrWhiteSpace(subString)) 
                return Historis;

            return Historis.Where(l => CultureInfo
                .InvariantCulture
                .CompareInfo
                .IndexOf(l, subString, CompareOptions.IgnoreCase) >= 0);
        }

        public History()
        {
            throw new Exception("U shell not pass.");
        }
    }
}
