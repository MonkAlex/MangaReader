using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MangaReader.Chapters
{
    /// <summary>
    /// Глава.
    /// </summary>
    public class Chapter
    {
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
            if (listOfImageLink == null)
                GetAllImagesLink();

            try
            {
                chapterFolder = Page.MakeValidPath(chapterFolder);
                if (!Directory.Exists(chapterFolder))
                    Directory.CreateDirectory(chapterFolder);

                Parallel.ForEach(listOfImageLink, link =>
                {
                    using (var webClient = new WebClient { Encoding = Encoding.UTF8 })
                    webClient.DownloadFile(link, string.Concat(chapterFolder, "\\", Path.GetFileName(link)));

                });
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                    Log.Add(this.Url + this.Name + ex.ToString());
            }
            catch (Exception ex)
            {
                Log.Add(this.Url + this.Name + ex.ToString());
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
            this.Volume = Convert.ToInt32(Regex.Match(url, @"vol[0-9]+").Value.Remove(0, 3));
            this.Number = Convert.ToInt32(Regex.Match(url, @"/[0-9]+").Value.Remove(0, 1));
        }
    }
}