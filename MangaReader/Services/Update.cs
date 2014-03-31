using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace MangaReader.Services
{
    class Update
    {
        private const string LinkToUpdate = "https://dl.dropboxusercontent.com/u/1945107/RMG/readmanga.exe";
        private const string LinkToVersion = "https://dl.dropboxusercontent.com/u/1945107/RMG/version.ini";
        private const string UpdateStarted = "update";
        private const string UpdateFinished = "updated";

        private const string UpdateFilename = "update.exe";
        private const string UpdateTempFilename = "update.it";
        private const string OriginalFilename = "MangaReader3.exe";
        private const string OriginalTempFilename = OriginalFilename + ".bak";

        /// <summary>
        /// Запуск обновления, вызываемый до инициализации программы.
        /// </summary>
        /// <remarks>Завершает обновление и удаляет временные файлы.</remarks>
        public static void Initialize()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Contains(UpdateStarted))
                Update.FinishUpdate();
            if (args.Contains(UpdateFinished))
                Update.Clean();
        }

        /// <summary>
        /// Проверка наличия обновления.
        /// </summary>
        /// <returns></returns>
        public static bool CheckUpdate()
        {
            using (var client = new WebClient())
            {
                var serverVersion = new Version(client.DownloadString(LinkToVersion));
                var clientVersion = Assembly.GetExecutingAssembly().GetName().Version;
                if (serverVersion.CompareTo(clientVersion) > 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Запуск обновления.
        /// </summary>
        public static void StartUpdate()
        {
            if (!Update.CheckUpdate())
                return;

            using (var client = new WebClient())
            {
                File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\" + UpdateFilename,
                    client.DownloadData(LinkToUpdate));
                File.Copy(UpdateFilename, UpdateTempFilename, true);
                var run = new Process()
                {
                    StartInfo =
                    {
                        Arguments = UpdateStarted,
                        FileName = UpdateFilename,
                        WorkingDirectory = Directory.GetCurrentDirectory()
                    }
                };
                run.Start();
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Завершение обновления и запуск обновленного приложения.
        /// </summary>
        public static void FinishUpdate()
        {
            File.Replace(UpdateTempFilename, OriginalFilename, OriginalTempFilename);
            var run = new Process()
            {
                StartInfo =
                {
                    Arguments = UpdateFinished,
                    FileName = OriginalFilename,
                    WorkingDirectory = Directory.GetCurrentDirectory()
                }
            };
            run.Start();
            Environment.Exit(1);
        }

        /// <summary>
        /// Удаление временных файлов.
        /// </summary>
        public static void Clean()
        {
            File.Delete(UpdateFilename);
            File.Delete(OriginalTempFilename);
            File.Delete(UpdateTempFilename);
        }
    }
}
