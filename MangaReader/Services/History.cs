﻿using System;
using System.Collections.Generic;
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
        private static List<MangaHistory> Historis = Serializer<List<MangaHistory>>.Load(HistoryPath);

        /// <summary>
        /// Добавление записи в историю.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public static void Add(string message)
        {
            lock (HistoryLock)
            {
                if (Historis.Any(l => l.Url == message))
                    return;

                Historis.Add(new MangaHistory(message));
            }
        }

        /// <summary>
        /// Сохранить историю локально.
        /// </summary>
        public static void Save()
        {
            lock (HistoryLock)
                Serializer<List<MangaHistory>>.Save(HistoryPath, Historis);
        }

        /// <summary>
        /// Сконвертировать в новый формат.
        /// </summary>
        public static void Convert()
        {
            if (Historis != null)
                return;

            var serializedStrings = Serializer<List<string>>.Load(HistoryPath);
            var isStringList = serializedStrings != null;

            var serializedMangaHistoris = Serializer<List<MangaHistory>>.Load(HistoryPath);
            var isMangaHistory = serializedMangaHistoris != null;


            var strings = File.Exists(HistoryPath) ? new List<string>(File.ReadAllLines(HistoryPath)) : new List<string>();

            if (!isMangaHistory && !isStringList)
                Historis = MangaHistory.CreateHistories(strings);
            if (!isMangaHistory && isStringList)
                Historis = MangaHistory.CreateHistories(serializedStrings);
            if (isMangaHistory && !isStringList)
                Historis = serializedMangaHistoris;

            Save();
        }

        /// <summary>
        /// Получить историю.
        /// </summary>
        /// <param name="subString">Подстрока, по которой будет получена история. Например, название манги.</param>
        /// <returns>Перечисление сообщений из истории.</returns>
        public static IEnumerable<MangaHistory> Get(string subString = "")
        {
            lock (HistoryLock)
                return string.IsNullOrWhiteSpace(subString) ?
                    Historis : 
                    Historis.Where(l => l.MangaUrl.Contains(subString)).ToList();
        }

        public History()
        {
            throw new Exception("U shell not pass.");
        }
    }
}
