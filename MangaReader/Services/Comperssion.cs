using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace MangaReader
{
    class Comperssion
    {
        /// <summary>
        /// Упаковка всех глав.
        /// </summary>
        /// <param name="message">Папка манги.</param>
        public static void ComperssChapters(string message)
        {
            if (Directory.Exists(message))
            {
                var volumes = Directory.GetDirectories(message);
                foreach (var volume in volumes)
                {
                    Directory.SetCurrentDirectory(volume);
                    var chapters = Directory.GetDirectories(".\\");
                    foreach (var chapter in chapters)
                    {
                        var acr = string.Concat(GetFolderName(message), "_", GetFolderName(volume), "_", GetFolderName(chapter), ".zip");
                        if (File.Exists(acr))
                            File.Delete(acr);
                        ZipFile.CreateFromDirectory(chapter, acr, CompressionLevel.NoCompression, false, Encoding.UTF8);
                        Directory.Delete(chapter, true);
                    }
                }
            }
        }

        /// <summary>
        /// Упаковка всех томов.
        /// </summary>
        /// <param name="message">Папка манги.</param>
        public static void ComperssVolumes(string message)
        {
            if (Directory.Exists(message))
            {
                Directory.SetCurrentDirectory(message);
                var volumes = Directory.GetDirectories(".\\");
                foreach (var volume in volumes)
                {
                    var acr = string.Concat(GetFolderName(message), "_", GetFolderName(volume), ".zip");
                    if (File.Exists(acr))
                        File.Delete(acr);
                    ZipFile.CreateFromDirectory(volume, acr, CompressionLevel.NoCompression, false, Encoding.UTF8);
                    Directory.Delete(volume, true);
                }
            }
        }

        /// <summary>
        /// Вернуть название папки.
        /// </summary>
        /// <param name="path">Путь к папке. Абсолютный или относительный.</param>
        /// <returns>Название папки. '.\Folder\' -> 'Folder'</returns>
        private static string GetFolderName(string path)
        {
            return path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                            StringSplitOptions.RemoveEmptyEntries)
                       .Last();
        }
    }
}
