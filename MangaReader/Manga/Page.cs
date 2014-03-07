using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MangaReader
{
    public static class Page
    {
        /// <summary>
        /// Получить текст страницы.
        /// </summary>
        /// <param name="url">Ссылка на страницу.</param>
        /// <returns>Исходный код страницы.</returns>
        public static string GetPage(string url)
        {
                try
                {
                    var webClient = new System.Net.WebClient { Encoding = Encoding.UTF8 };
                    return webClient.DownloadString(url);
                }
                catch (Exception ex)
                {
                    Log.Add(ex.ToString());
                    return string.Empty;
                }
        }

        /// <summary>
        /// Очистка имени файла от недопустимых символов.
        /// </summary>
        /// <param name="name">Имя файла.</param>
        /// <returns>Исправленное имя файла.</returns>
        public static string MakeValidFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }

        /// <summary>
        /// Очистка пути от недопустимых символов.
        /// </summary>
        /// <param name="name">Путь.</param>
        /// <returns>Исправленный путь.</returns>
        public static string MakeValidPath(string name)
        {
            //TODO: Символ ":" в абсолютном пути должен быть не более одного раза, и то не всегда.
            if (!Path.IsPathRooted(name))
                name = name.Replace(":", "");
            string invalidChars = Regex.Escape(new string(Path.GetInvalidPathChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
