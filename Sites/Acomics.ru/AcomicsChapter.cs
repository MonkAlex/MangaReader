using System;
using System.Text.RegularExpressions;

namespace Acomics
{
  /// <summary>
  /// Глава.
  /// </summary>
  public class AcomicsChapter : MangaReader.Core.Manga.Chapter
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
    public AcomicsChapter(Uri uri, string desc)
      : this(uri)
    {
      this.Name = desc;
    }

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="uri">Ссылка на главу.</param>
    public AcomicsChapter(Uri uri)
      : base(uri)
    {
      this.Uri = uri;
      this.Number = GetChapterNumber(uri.OriginalString);
    }

    protected AcomicsChapter()
    {
      
    }

    #endregion

  }
}