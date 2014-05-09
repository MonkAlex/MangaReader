using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MangaReader
{
    public static class Page
    {

        /// <summary>
        /// Сжать изображение.
        /// </summary>
        /// <param name="image">Изображение.</param>
        /// <returns>Сжатое изображение.</returns>
        public static byte[] GetThumbnail(byte[] image)
        {
            var memory = new MemoryStream();
            Image
                .FromStream(new MemoryStream(image))
                .GetThumbnailImage(128, 200, null, new IntPtr())
                .Save(memory, ImageFormat.Png);
            return memory.ToArray();
        }

        /// <summary>
        /// Получить расширение изображения.
        /// </summary>
        /// <param name="image">Изображение.</param>
        /// <returns>Строка с типом картинки.</returns>
        public static string GetImageExtension(byte[] image)
        {
            var created = Image.FromStream(new MemoryStream(image));
            return new ImageFormatConverter().ConvertToString(created.RawFormat).ToLower();
        }

        /// <summary>
        /// Скачать файл.
        /// </summary>
        /// <param name="url">Ссылка на файл.</param>
        /// <returns>Содержимое файла.</returns>
        public static byte[] DownloadFile(string url)
        {
            Byte[] result;
            WebResponse response;
            var request = WebRequest.Create(url);

            try
            {
                response = request.GetResponse();
                var ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                result = ms.ToArray();
                ms.Dispose();
            }
            catch
            {
                return null;
            }

            return response.ContentLength == result.LongLength ? result : null;
        }

        /// <summary>
        /// Получить текст страницы.
        /// </summary>
        /// <param name="url">Ссылка на страницу.</param>
        /// <returns>Исходный код страницы.</returns>
        public static string GetPage(string url)
        {
                try
                {
                    var webClient = new WebClient { Encoding = Encoding.UTF8 };
                    return webClient.DownloadString(url);
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
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
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }

        /// <summary>
        /// Очистка пути от недопустимых символов.
        /// </summary>
        /// <param name="name">Путь.</param>
        /// <returns>Исправленный путь.</returns>
        public static string MakeValidPath(string name)
        {
            const string replacement = "_";
            var matchesCount = Regex.Matches(name, @":\\").Count;
            string correctName;
            if (matchesCount > 0)
            {
                var regex = new Regex(@":", RegexOptions.RightToLeft);
                correctName = regex.Replace(name, replacement, regex.Matches(name).Count - matchesCount);
            }
            else
                correctName = name.Replace(":", replacement);

            var invalidChars = Regex.Escape(new string(Path.GetInvalidPathChars()));
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(correctName, invalidRegStr, replacement);
        }
    }
}
