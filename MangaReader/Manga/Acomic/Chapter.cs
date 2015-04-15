using System;
using System.Text.RegularExpressions;

namespace MangaReader.Manga.Acomic
{
  /// <summary>
  /// Глава.
  /// </summary>
  public class Chapter : MangaReader.Manga.Chapter
  {

    #region Конструктор

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="uri">Ссылка на главу.</param>
    /// <param name="desc">Описание главы.</param>
    public Chapter(Uri uri, string desc)
      : this(uri)
    {
      this.Name = desc;
    }

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="uri">Ссылка на главу.</param>
    public Chapter(Uri uri)
      : base(uri)
    {
      this.Uri = uri;
      this.Number = Convert.ToInt32(Regex.Match(uri.OriginalString, @"/[-]?[0-9]+", RegexOptions.RightToLeft).Value.Remove(0, 1));
    }

    #endregion

  }
}