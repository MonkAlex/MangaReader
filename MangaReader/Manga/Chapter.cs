using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MangaReader
{
    /// <summary>
    /// Глава.
    /// </summary>
    public class Chapter
    {
        /// <summary>
        /// Количество запусков загрузки.
        /// </summary>
        private int counter;

        /// <summary>
        /// Хранилище ссылок на изображения.
        /// </summary>
        private List<string> listOfImageLink;

        /// <summary>
        /// Название главы.
        /// </summary>
        public string Name;

        /// <summary>
        /// Ссылка на главу.
        /// </summary>
        public string Url;

        /// <summary>
        /// Номер главы.
        /// </summary>
        public int Number;

        /// <summary>
        /// Номер тома.
        /// </summary>
        public int Volume;

        /// <summary>
        /// Скачать главу.
        /// </summary>
        /// <param name="chapterFolder">Папка для файлов.</param>
        public void Download(string chapterFolder)
        {
            if (counter > 3)
                throw new Exception(string.Format("Load failed after {0} counts.", counter));

            if (listOfImageLink == null)
                GetAllImagesLink();

            var chLink = string.Empty;
            var chFile = string.Empty;

            try
            {
                chapterFolder = Page.MakeValidPath(chapterFolder);
                if (!Directory.Exists(chapterFolder))
                    Directory.CreateDirectory(chapterFolder);

                Parallel.ForEach(listOfImageLink,
                    () => new WebClient(),
                    (link, loopstate, webclient) =>
                {
                    chLink = link;
                    chFile = string.Concat(chapterFolder, "\\", Path.GetFileName(chLink));
                    File.WriteAllBytes(chFile, webclient.DownloadData(chLink));
                    return webclient;
                },
                    webclient => { });

                History.Add(this.Url);
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                    Log.Exception(ex, this.Url, this.Name, chLink, chFile);
                ++counter;
                Download(chapterFolder);
            }
            catch (Exception ex)
            {
                Log.Exception(ex, this.Url, this.Name);
            }
        }

        /// <summary>
        /// Заполнить хранилище ссылок.
        /// </summary>
        private void GetAllImagesLink()
        {
            this.listOfImageLink = Getter.GetImagesLink(this.Url);
        }

        /// <summary>
        /// Глава манги.
        /// </summary>
        /// <param name="url">Ссылка на главу.</param>
        /// <param name="desc">Описание главы.</param>
        public Chapter(string url, string desc)
        {
            this.Url = url;
            this.Name = desc;
            this.counter = 0;
            this.Volume = Convert.ToInt32(Regex.Match(url, @"vol[-]?[0-9]+").Value.Remove(0, 3));
            this.Number = Convert.ToInt32(Regex.Match(url, @"/[-]?[0-9]+", RegexOptions.RightToLeft).Value.Remove(0, 1));
        }
    }
}