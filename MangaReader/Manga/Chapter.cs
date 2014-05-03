using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MangaReader
{
    /// <summary>
    /// Глава.
    /// </summary>
    public class Chapter
    {
        #region Свойства

        /// <summary>
        /// Количество перезапусков загрузки.
        /// </summary>
        private int restartCounter;

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

        #endregion

        #region Методы

        /// <summary>
        /// Скачать главу.
        /// </summary>
        /// <param name="chapterFolder">Папка для файлов.</param>
        public void Download(string chapterFolder)
        {
            if (restartCounter > 3)
                throw new Exception(string.Format("Load failed after {0} counts.", restartCounter));

            if (listOfImageLink == null)
                GetAllImagesLink();

            try
            {
                chapterFolder = Page.MakeValidPath(chapterFolder);
                if (!Directory.Exists(chapterFolder))
                    Directory.CreateDirectory(chapterFolder);

                Parallel.ForEach(listOfImageLink, link =>
                {
                    var file = Page.DownloadFile(link);
                    if (file == null)
                        throw new Exception("Restart chapter download, downloaded file is corrupted.");

                    var fileName = listOfImageLink
                        .FindIndex(l => l == link)
                        .ToString()
                        .PadLeft(4, '0') + 
                        "." + 
                        Page.GetImageExtension(file);

                    File.WriteAllBytes(string.Concat(chapterFolder, "\\", fileName), file);
                });

                History.Add(this.Url);
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                    Log.Exception(ex, this.Url, this.Name);
                ++restartCounter;
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

        #endregion

        #region Конструктор

        /// <summary>
        /// Глава манги.
        /// </summary>
        /// <param name="url">Ссылка на главу.</param>
        /// <param name="desc">Описание главы.</param>
        public Chapter(string url, string desc)
        {
            this.Url = url;
            this.Name = desc;
            this.restartCounter = 0;
            this.Volume = Convert.ToInt32(Regex.Match(url, @"vol[-]?[0-9]+").Value.Remove(0, 3));
            this.Number = Convert.ToInt32(Regex.Match(url, @"/[-]?[0-9]+", RegexOptions.RightToLeft).Value.Remove(0, 1));
        }

        #endregion

    }
}