using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Services;

namespace MangaReader.Manga.Acomic
{
  /// <summary>
  /// Манга.
  /// </summary>
  public class Acomics : Mangas
  {
    #region Свойства

    public new static Guid Type { get { return Guid.Parse("F090B9A2-1400-4F5E-B298-18CD35341C34"); } }

    public override List<Compression.CompressionMode> AllowedCompressionModes
    {
      get
      {
        return base.AllowedCompressionModes
          .Where(m => this.HasVolumes && (Equals(m, Compression.CompressionMode.Volume) || Equals(m, Compression.CompressionMode.Chapter)) ||
            this.HasChapters && Equals(m, Compression.CompressionMode.Volume) ||
            Equals(m, Compression.CompressionMode.Manga))
          .ToList();
      }
    }

    #endregion

    #region Методы

    /// <summary>
    /// Обновить информацию о манге - название, главы, обложка.
    /// </summary>
    public override void Refresh()
    {
      var newName = Getter.GetMangaName(this.Uri);
      if (string.IsNullOrWhiteSpace(newName))
        Log.Add("Не удалось получить имя манги, текущее название = " + this.ServerName);
      else if (newName != this.ServerName)
        this.ServerName = newName;

      Getter.UpdateContentType(this);
      OnPropertyChanged("IsCompleted");
    }

    protected override void UpdateContent()
    {
      this.Pages.Clear();
      this.Chapters.Clear();
      this.Volumes.Clear();

      Getter.UpdateContent(this);

      base.UpdateContent();
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Открыть мангу.
    /// </summary>
    /// <param name="url">Ссылка на мангу.</param>
    public Acomics(Uri url)
      : this()
    {
      this.Uri = url;
      this.Refresh();
    }

    public Acomics()
      : base()
    {
    }

    #endregion
  }
}
