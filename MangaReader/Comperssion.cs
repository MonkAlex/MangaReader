using System.IO;
using System.IO.Compression;
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
                    var chapters = Directory.GetDirectories(volume);
                    foreach (var chapter in chapters)
                    {
                        var acr = chapter + ".zip";
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
                var volumes = Directory.GetDirectories(message);
                foreach (var volume in volumes)
                {
                    var acr = volume + ".zip";
                    if (File.Exists(acr))
                        File.Delete(acr);
                    ZipFile.CreateFromDirectory(volume, acr, CompressionLevel.NoCompression, false, Encoding.UTF8);
                    Directory.Delete(volume, true);
                }
            }
        }
    }
}
