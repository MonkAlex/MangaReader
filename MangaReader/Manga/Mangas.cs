using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MangaReader.Manga
{
    [XmlInclude(typeof(Readmanga)), XmlInclude(typeof(Acomics))]
    public abstract class Mangas : INotifyPropertyChanged, IDownloadable
    {
        #region Свойства

        /// <summary>
        /// Название манги.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ссылка на мангу.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Статус манги.
        /// </summary>
        public abstract string Status { get; set; }

        /// <summary>
        /// Нужно ли обновлять мангу.
        /// </summary>
        public abstract bool NeedUpdate { get; set; }

        /// <summary>
        /// Статус корректности манги.
        /// </summary>
        public abstract bool IsValid { get; }

        /// <summary>
        /// Статус перевода.
        /// </summary>
        public abstract string IsCompleted { get; }

         #endregion

        #region DownloadProgressChanged

        public abstract bool IsDownloaded { get; }
        
        public abstract double Downloaded { get; set; }
        
        /// <summary>
        /// Папка манги.
        /// </summary>
        public  string Folder 
        {
            get { return Page.MakeValidPath(Settings.DownloadFolder + "\\" + this.Name); }
        }

        public abstract event EventHandler<Mangas> DownloadProgressChanged;

        public abstract void Download(string mangaFolder = null, string volumePrefix = null, string chapterPrefix = null);

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Обновить информацию о манге - название, главы, обложка.
        /// </summary>
        public abstract void Refresh();

        /// <summary>
        /// Упаковка манги.
        /// </summary>
        public abstract void Compress();

        /// <summary>
        /// Создать мангу по ссылке.
        /// </summary>
        /// <param name="url">Ссылка на мангу.</param>
        /// <returns>Манга.</returns>
        public static Mangas Create(string url)
        {
            if (url.Contains("readmanga.me") || url.Contains("adultmanga.ru"))
                return new Readmanga(url);
            if (url.Contains("acomics.ru"))
                return new Acomics(url);
            return null;
        }
        
        #endregion
    }
}
