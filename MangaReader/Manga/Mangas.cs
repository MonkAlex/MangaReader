using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MangaReader.Manga
{
  [XmlInclude(typeof(Grouple.Readmanga)), XmlInclude(typeof(Acomic.Acomics))]
  public abstract class Mangas : Entity.Entity, INotifyPropertyChanged, IDownloadable
  {
    #region Свойства

    protected static internal string Type { get { return "Type"; } }

    /// <summary>
    /// Название манги.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Ссылка на мангу.
    /// </summary>
    public virtual string Url { get; set; }

    /// <summary>
    /// Статус манги.
    /// </summary>
    public abstract string Status { get; set; }

    public virtual List<string> Doubles { get; set; }

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

    public abstract string Folder { get; }

    public abstract event EventHandler<Mangas> DownloadProgressChanged;

    public abstract void Download(string mangaFolder = null, string volumePrefix = null, string chapterPrefix = null);

    #endregion

    #region INotifyPropertyChanged

    public virtual event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string property)
    {
      var handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(property));
      }
    }

    #endregion

    #region Equals

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;

      var manga = obj as Mangas;
      return manga == null ? base.Equals(obj) : this.Url.Equals(manga.Url);
    }

    public override int GetHashCode()
    {
      return this.Url.GetHashCode()*this.Name.GetHashCode();
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
      Mangas manga = null;

      if (url.Contains("readmanga.me") || url.Contains("adultmanga.ru"))
        manga = new Grouple.Readmanga(url);
      if (url.Contains("acomics.ru"))
        manga = new Acomic.Acomics(url);

      if (manga != null)
        manga.Save();

      return manga;
    }

    #endregion
  }
}
