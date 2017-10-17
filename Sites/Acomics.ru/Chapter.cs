using System;
using System.Text.RegularExpressions;

namespace Acomics
{
  /// <summary>
  /// Глава.
  /// </summary>
  public class Chapter : MangaReader.Core.Manga.Chapter
  {
    internal static int GetChapterNumber(string uri)
    {
      return Convert.ToInt32(Regex.Match(uri, @"/[-]?[0-9]+", RegexOptions.RightToLeft).Value.Remove(0, 1));
    }

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
      this.Number = GetChapterNumber(uri.OriginalString);
    }

    #endregion

  }
}