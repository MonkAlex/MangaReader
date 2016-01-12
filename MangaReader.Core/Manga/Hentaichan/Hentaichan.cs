using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Services;

namespace MangaReader.Manga.Hentaichan
{
  public class Hentaichan : Mangas
  {
    public new static Guid Type { get { return Guid.Parse("6F2A3ACC-70B2-4FF3-9BCB-0E3D15971FDE"); } }

    public override List<Compression.CompressionMode> AllowedCompressionModes
    {
      get { return base.AllowedCompressionModes.Where(m => !Equals(m, Compression.CompressionMode.Chapter)).ToList(); }
    }

    public override void Refresh()
    {
      base.Refresh();

      var newName = Getter.GetMangaName(this.Uri);
      if (string.IsNullOrWhiteSpace(newName))
        Log.Add("Не удалось получить имя манги, текущее название = " + this.ServerName);
      else if (newName != this.ServerName)
        this.ServerName = newName;
    }

    public override bool IsValid()
    {
      return base.IsValid() && !Library.LibraryMangas.Any(m => !Equals(m, this) &&
                                                               Equals(m.ServerName, this.ServerName) &&
                                                               Equals(m.GetType().TypeProperty(), Type));
    }

    protected override void UpdateContent()
    {
      this.Pages.Clear();
      this.Chapters.Clear();
      this.Volumes.Clear();

      Getter.UpdateContent(this);

      base.UpdateContent();
    }

    #region Конструктор

    /// <summary>
    /// Открыть мангу.
    /// </summary>
    /// <param name="url">Ссылка на мангу.</param>
    public Hentaichan(Uri url)
      : this()
    {
      this.Uri = url;
      this.Refresh();
    }

    public Hentaichan()
      : base()
    {
    }

    #endregion

  }
}
